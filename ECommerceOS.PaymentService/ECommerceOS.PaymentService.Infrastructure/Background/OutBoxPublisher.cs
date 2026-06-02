using System.Collections.Concurrent;
using Dapper;
using ECommerceOS.Shared.Contracts.Interfaces;
using ECommerceOS.Shared.Contracts.Messaging.Payment;
using MassTransit;
using Npgsql;
using Quartz;

namespace ECommerceOS.PaymentService.Infrastructure.Background;

public class OutBoxPublisher(
    ILogger<OutBoxPublisher> logger,
    NpgsqlDataSource dataSource,
    IProducer<string, PaymentEvent> producer,
    IAsyncDeserializer<IIntegrationEvent> deserializer)
    : IJob
{
    private const int BatchSize = 20;
    private readonly SerializationContext _context = new();
    
    public async Task Execute(IJobExecutionContext context)
    {
        await using var connection = await dataSource.OpenConnectionAsync(context.CancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(context.CancellationToken);
        
        var messages = (await connection.QueryAsync<OutboxMessage>(
                """
                SELECT
                    message_id AS MessageId,
                    type AS Type,
                    integration_event AS IntegrationEvent,
                    processed_on AS ProcessedOn,
                    attempts AS Attempts,
                    error AS ErrorMessage,
                    created_at AS CreatedAt
                FROM outbox_messages
                WHERE processed_on IS NULL
                ORDER BY created_at
                LIMIT @BatchSize
                FOR UPDATE SKIP LOCKED
                """,
                new { BatchSize }, transaction))
            .AsList();

        if (messages.Count == 0)
        {
            await transaction.CommitAsync(context.CancellationToken);
            return;
        }

        var updateQueue = new ConcurrentQueue<OutboxUpdate>();
        var publishTasks = messages.Select(async outbox =>
        {
            try
            {
                var identityEvent = new PaymentEvent
                {
                    MessageId = outbox.MessageId,
                    Type = outbox.Type,
                    Attempts = outbox.Attempts,
                    CreatedAt = outbox.CreatedAt,
                    IntegrationEvent = await deserializer.DeserializeAsync(outbox.IntegrationEvent, false, _context)
                };

                var message = new Message<string, PaymentEvent>
                {
                    Key = "catalog-event",
                    Value = identityEvent
                };
                
                await producer.ProduceAsync("catalog-event", message, context.CancellationToken);
                
                logger.LogInformation($"Outbox published: {outbox.MessageId}");
                
                updateQueue.Enqueue(new OutboxUpdate
                {
                    Id = outbox.MessageId,
                    ProcessedOn = DateTime.UtcNow,
                    Attempts = ++outbox.Attempts
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Outbox publishing failed");
                updateQueue.Enqueue(new OutboxUpdate
                {
                    Id = outbox.MessageId,
                    Attempts = ++outbox.Attempts,
                    Error = ex.Message
                });
            }
        });
        
        await Task.WhenAll(publishTasks);

        if (updateQueue.IsEmpty)
        {
            await transaction.CommitAsync(context.CancellationToken);
            return;
        }

        var updateSql =
            """
            UPDATE outbox_messages
            SET processed_on = v.processed_on,
                error = v.error,
                attempts = v.attempts
            FROM (VALUES {0})
            AS v(id, processed_on, error, attempts)
            WHERE outbox_messages.message_id = v.id::uuid
            """;

        var updates = updateQueue.ToList();
        var valuesList = string.Join(",",
            updates.Select((_, i) => $"(@Id{i}, @ProcessedOn{i}, @Error{i}, @Attempts{i})"));

        var parameters = new DynamicParameters();

        for (var i = 0; i < updates.Count; i++)
        {
            parameters.Add($"Id{i}", updates[i].Id.ToString());
            parameters.Add($"ProcessedOn{i}", updates[i].ProcessedOn);
            parameters.Add($"Error{i}", updates[i].Error);
            parameters.Add($"Attempts{i}", updates[i].Attempts);
        }
        
        var formattedSql = string.Format(updateSql, valuesList);

        await connection.ExecuteAsync(formattedSql, parameters, transaction: transaction);
        await transaction.CommitAsync(context.CancellationToken); 
        
    }

    private readonly struct OutboxUpdate
    {
        public required Guid Id { get; init; }
        public required short Attempts { get; init; }
        public string? Error { get; init; }
        public DateTime? ProcessedOn { get; init; }
    }
}
