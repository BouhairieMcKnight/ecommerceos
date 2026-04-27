using Avro;
using Avro.Specific;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using NJsonSchema.Infrastructure;
using Schema = Avro.Schema;

namespace ECommerceOS.AppHost;

public class SchemaRegistryHealthCheck(
    SchemaRegistryHealthCheckOptions options
    ) 
    : IHealthCheck, IDisposable
{
    private ISchemaRegistryClient? _schemaRegistryClient;
    
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            _schemaRegistryClient ??= new CachedSchemaRegistryClient(options.Configuration);
            var schemaString = "{\n \"namespace\": \"io.confluent.examples.clients.basicavro\",\n \"type\": \"record\",\n \"name\": \"Payment\",\n \"fields\": [\n     {\"name\": \"id\", \"type\": \"string\"},\n     {\"name\": \"amount\", \"type\": \"double\"}\n ]\n}";
            var subject = "my-topic-value";

            await _schemaRegistryClient.RegisterSchemaAsync(subject, new Confluent.SchemaRegistry.Schema(schemaString, SchemaType.Avro)).ConfigureAwait(false);
            
            var subjects = await _schemaRegistryClient.GetAllSubjectsAsync().ConfigureAwait(false);
            
            if (!subjects.Any() || !subjects.Contains(subject))
            {
                return new HealthCheckResult(context.Registration.FailureStatus, "Could not get subjects");
            }

        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, "Exception on health check was thrown");
        }
        
        return HealthCheckResult.Healthy();
    }

    public void Dispose()
    {
        _schemaRegistryClient?.Dispose();
    }
}
