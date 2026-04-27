using Aspire.Hosting.Yarp;
using Aspire.Hosting.Yarp.Transforms;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using ECommerceOS.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var kafka = builder.AddKafka("kafka")
    .WithKafkaUI();

var schemaRegistry = builder.AddSchemaRegistry("schemaRegistry")
    .WithReference(kafka)
    .WaitFor(kafka);

builder.Eventing.Subscribe<ResourceReadyEvent>(kafka.Resource, async (e, ct) =>
{
    var connectionString = await kafka.Resource.ConnectionStringExpression.GetValueAsync(ct);

    var config = new AdminClientConfig
    {
        BootstrapServers = connectionString
    };
    
    using var client = new AdminClientBuilder(config).Build();
    try
    {
        await client.CreateTopicsAsync([
            new TopicSpecification { Name = "catalog-event", NumPartitions = 1, ReplicationFactor = 1 },
            new TopicSpecification { Name = "payment-event", NumPartitions = 1, ReplicationFactor = 1 },
            new TopicSpecification { Name = "order-event", NumPartitions = 1, ReplicationFactor = 1 },
            new TopicSpecification { Name = "identity-event", NumPartitions = 1, ReplicationFactor = 1 }
        ]);
    }
    catch (CreateTopicsException ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin(pgAdmin =>
    {
        pgAdmin.WithHostPort(5050);
    });

var blobs = builder.AddAzureStorage("AzureStorage")
    .RunAsEmulator(azurite =>
    {
        azurite.WithEndpoint(3100, targetPort: 3100);
    }).AddBlobs("blobs");

var identityDb = postgres.AddDatabase("identitydb");
var paymentDb = postgres.AddDatabase("paymentdb");
var catalogDb = postgres.AddDatabase("catalogdb");
var orderDb = postgres.AddDatabase("orderdb");

var authService = builder.AddProject<Projects.ECommerceOS_AuthService_WebApi>("authservice")
    .WithReference(identityDb)
    .WaitFor(identityDb)
    .WithReference(kafka)
    .WaitFor(kafka)
    .WithReference(schemaRegistry)
    .WaitFor(schemaRegistry)
    .WithReference(cache)
    .WaitFor(cache);


var paymentService = builder.AddProject<Projects.ECommerceOS_PaymentService_WebApi>("paymentservice")
    .WithReference(authService)
    .WithReference(paymentDb)
    .WithReference(schemaRegistry)
    .WaitFor(schemaRegistry)
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(kafka)
    .WaitFor(kafka);


var catalogService = builder.AddProject<Projects.ECommerceOS_CatalogService_WebApi>("catalogservice")
    .WithReference(catalogDb)
    .WaitFor(catalogDb)
    .WithReference(authService)
    .WithReference(paymentService)
    .WaitFor(paymentService)
    .WithReference(kafka)
    .WaitFor(kafka)
    .WithReference(schemaRegistry)
    .WaitFor(schemaRegistry)
    .WithReference(blobs)
    .WaitFor(blobs)
    .WithReference(cache)
    .WaitFor(cache);

var orderService = builder.AddProject<Projects.ECommerceOS_OrderService_WebApi>("orderservice")
    .WithReference(authService)
    .WithReference(orderDb)
    .WaitFor(orderDb)
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(catalogService)
    .WaitFor(catalogService)
    .WithReference(kafka)
    .WaitFor(kafka)
    .WithReference(schemaRegistry)
    .WaitFor(schemaRegistry);

var gateway = builder.AddYarp("gateway")
    .WithHttpsEndpoint(port: 8001, targetPort: 8001)
    .WithConfiguration(yarp =>
    {
        yarp.AddRoute("/api/payment/{**catch-all}", paymentService)
            .WithTransformPathRemovePrefix("/api/payment/");

        yarp.AddRoute("/webhook", paymentService)
            .WithTransformPathRemovePrefix("/webhook")
            .WithTransformPathSet("/stripe/webhook-endpoint");

        yarp.AddRoute("api/auth/{**catch-all}", authService)
            .WithTransformPathRemovePrefix("/api");
        
        yarp.AddRoute("/api/order/{**catch-all}", orderService)
            .WithTransformPathRemovePrefix("/api");
        
        yarp.AddRoute("api/catalog/{**catch-all}", catalogService)
            .WithTransformPathRemovePrefix("/api/catalog");
    });

builder.Build().Run();