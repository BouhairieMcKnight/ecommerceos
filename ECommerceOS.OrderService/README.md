# ECommerceOS.OrderService

`ECommerceOS.OrderService` owns the order boundary. It exposes order queries and deletion endpoints while coordinating persistence, caching, event publication, and inventory reservation against the catalog service.

## Solution layout

- `ECommerceOS.OrderService.Application` contains order use cases
- `ECommerceOS.OrderService.Domain` contains order domain types
- `ECommerceOS.OrderService.Domain.Tests` contains domain tests
- `ECommerceOS.OrderService.Infrastructure` contains persistence, messaging, caching, gRPC clients, middleware, serialization, and background jobs
- `ECommerceOS.OrderService.Presentation` contains HTTP endpoint groups
- `ECommerceOS.OrderService.WebApi` is the service host

## API surface

The service exposes HTTP endpoints under `/orders`:

- get order by id
- get paginated orders
- delete order by id

The service also publishes order integration events and calls the catalog reservation gRPC service through the shared `Reserve` protobuf contract.

## Infrastructure dependencies

- PostgreSQL database: `orderdb`
- Kafka producer for `order-event`
- Schema Registry for Avro serialization
- Redis for distributed cache and backplane support
- Quartz for scheduled outbox publishing
- gRPC client to `CatalogService` inventory reservation
- SMTP configuration for outbound email paths

## Configuration

Development configuration lives in `ECommerceOS.OrderService.WebApi/appsettings.Development.json`.

Key sections:

- `ConnectionStrings:orderdb`
- `ConnectionStrings:kafka`
- `ConnectionStrings:schemaRegistry`
- `ConnectionStrings:cache`
- `SmtpClientOptions`

## Run locally

Preferred:

```bash
dotnet run --project ../ECommerceOS.AppHost
```

Standalone:

```bash
dotnet run --project ECommerceOS.OrderService.WebApi
```

Standalone execution requires PostgreSQL, Kafka, Schema Registry, Redis, and the downstream catalog gRPC surface.

## Test

```bash
dotnet test ECommerceOS.OrderService.slnx
```
