using Aspire.Hosting.Eventing;
using Confluent.SchemaRegistry;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace ECommerceOS.AppHost;

public sealed class SchemaRegistryResource([ResourceName] string name)
    : ContainerResource(name), IResourceWithConnectionString
{
    internal const string PrimaryEndpointName = "tcp";
    internal const string InternalEndpointName = "internal";

    public EndpointReference InternalEndpoint =>
        field ??= new(this, InternalEndpointName);
    
    public EndpointReference PrimaryEndpoint =>
        field ??= new(this, PrimaryEndpointName);

    public EndpointReferenceExpression Port => PrimaryEndpoint.Property(EndpointProperty.Port);
    
    public EndpointReferenceExpression Host => PrimaryEndpoint.Property(EndpointProperty.Host);
    
    public EndpointReferenceExpression TargetPort => PrimaryEndpoint.Property(EndpointProperty.TargetPort);

    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create($"{PrimaryEndpoint.Property(EndpointProperty.HostAndPort)}");

    IEnumerable<KeyValuePair<string, ReferenceExpression>> IResourceWithConnectionString.GetConnectionProperties()
    {
        yield return new("Host", ReferenceExpression.Create($"{Host}"));
        yield return new("Port", ReferenceExpression.Create($"{Port}"));
        yield return new("TargetPort", ReferenceExpression.Create($"{TargetPort}"));
    }
}


public static class SchemaRegistryResourceExtensions
{
    private const int SchemaRegistryPort = 8081;
    private const int SchemaRegistryInternalPort = 8082;
    
    public static IResourceBuilder<SchemaRegistryResource> AddSchemaRegistry(
        this IDistributedApplicationBuilder builder, [ResourceName] string name, int? port = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(name);

        var referenceEnvironment = new ReferenceEnvironmentInjectionAnnotation(ReferenceEnvironmentInjectionFlags.ConnectionString);
        
        var schemaRegistry = new SchemaRegistryResource(name);

        string? connectionString = null;

        builder.Eventing.Subscribe<ConnectionStringAvailableEvent>(schemaRegistry, async (@event, ct) =>
        {
            connectionString = await schemaRegistry.ConnectionStringExpression.GetValueAsync(ct);

            if (connectionString == null)
            {
                throw new DistributedApplicationException(
                    $"ConnectionStringAvailableEvent was published for the '{schemaRegistry.Name}' resource but the connection string was null");
            }
        });
        
        var healthCheckKey = $"{name}_check";
        
        var healthCheckRegistration = new HealthCheckRegistration(
            healthCheckKey,
            sp =>
            {
                var options = new SchemaRegistryHealthCheckOptions();
                options.Configuration = new SchemaRegistryConfig();
                options.Configuration.Url = connectionString ?? throw new InvalidOperationException("Connection string is unavailable");
                return new SchemaRegistryHealthCheck(options);
            },
            failureStatus: default,
            tags: default);
        
        builder.Services.AddHealthChecks().Add(healthCheckRegistration);

        return builder.AddResource(schemaRegistry)
            .WithEndpoint(targetPort: SchemaRegistryPort, port: port, name: SchemaRegistryResource.PrimaryEndpointName)
            .WithEndpoint(targetPort: SchemaRegistryInternalPort, name: SchemaRegistryResource.InternalEndpointName)
            .WithImage(SchemaRegistryContainerImageTags.Image, SchemaRegistryContainerImageTags.Tag)
            .WithImageRegistry(SchemaRegistryContainerImageTags.Registry)
            .WithAnnotation(referenceEnvironment)
            .WithEnvironment(context => ConfigureSchemaRegistryContainer(context, schemaRegistry))
            .OnBeforeResourceStarted(static (resource, @event, ct) =>
            {
                var environmentAnnotation = new EnvironmentCallbackAnnotation(static context =>
                {
                    var connections = context.EnvironmentVariables.Values
                        .OfType<ConnectionStringReference>()
                        .ToArray();

                    if (!connections.Any())
                    {
                        throw new DistributedApplicationException("Could not find any kafka broker connection strings");
                    }
                    
                    ReferenceExpressionBuilder builder = new();
                    for (var i = 0; i < connections.Length; i++)
                    {
                        var endpointResource = connections[i].Resource as KafkaServerResource;
                        
                        if (i != 0)
                        {
                            builder.AppendLiteral(",");
                        }
                        
                        var endpoint = context.ExecutionContext.IsRunMode 
                                ? endpointResource?.InternalEndpoint
                                : endpointResource?.PrimaryEndpoint; 
                        
                        builder.Append($"PLAINTEXT://{endpointResource?.Name}:{endpoint?.Property(EndpointProperty.Port)}");
                    }

                    context.EnvironmentVariables["SCHEMA_REGISTRY_KAFKASTORE_BOOTSTRAP_SERVERS"] = builder.Build();
                });
                
                resource.Annotations.Add(environmentAnnotation);

                return Task.CompletedTask;
            })
            .WithHealthCheck(healthCheckKey);
    }
    
    private static Task ConfigureSchemaRegistryContainer(EnvironmentCallbackContext context, SchemaRegistryResource schemaRegistry)
    {
        context.EnvironmentVariables["SCHEMA_REGISTRY_HOST_NAME"] = ReferenceExpression.Create($"{schemaRegistry.Host}");
        
        context.EnvironmentVariables["SCHEMA_REGISTRY_LISTENERS"] = ReferenceExpression
            .Create($"http://0.0.0.0:{schemaRegistry.Port}");

        return Task.CompletedTask;
    }
}
