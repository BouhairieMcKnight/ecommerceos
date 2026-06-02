# ECommerceOS.CatalogService

`ECommerceOS.CatalogService` owns catalog browsing and shopping-cart workflows. It handles products, categories, cart state, checkout handoff, media uploads, and the gRPC inventory-reservation surface used by other services.

## Solution layout

- `ECommerceOS.CatalogService.Application` contains product, category, and cart use cases
- `ECommerceOS.CatalogService.Domain` contains catalog aggregates and domain rules
- `ECommerceOS.CatalogService.Domain.Tests` contains domain-focused tests
- `ECommerceOS.CatalogService.Infrastructure` contains persistence, messaging, caching, blob storage, external clients, and background jobs
- `ECommerceOS.CatalogService.Presentation` contains HTTP groups, gRPC services, and exception handling
- `ECommerceOS.CatalogService.WebApi` is the service host

## HTTP and gRPC surface

HTTP route groups:

- `/products`
- `/categories`
- `/cart`

The HTTP surface includes:

- create product
- get product by id
- get paginated products
- upload product images
- delete product
- get category hierarchy
- get cart
- add item to cart
- submit checkout

gRPC surface:

- `ReserveInventory` via the `Reserve` contract in `ECommerceOS.Shared/Contracts/ProtoBuf/Reserve.proto`

## Infrastructure dependencies

- PostgreSQL database: `catalogdb`
- Azure Blob Storage connection: `blobs`
- Kafka producer for `catalog-event`
- Kafka consumer for `order-event`
- Schema Registry for Avro serialization
- Redis plus session state for cart workflows
- Quartz for scheduled outbox publishing
- gRPC client to `PaymentService` checkout flow
- JWT authentication configuration

## Configuration

Development configuration lives in `ECommerceOS.CatalogService.WebApi/appsettings.Development.json`.

Key sections:

- `ConnectionStrings:catalogdb`
- `ConnectionStrings:kafka`
- `ConnectionStrings:schemaRegistry`
- `ConnectionStrings:cache`
- `ConnectionStrings:blobs`
- `JwtOptions`
- `SmtpClientOptions`

## Run locally

Preferred:

```bash
dotnet run --project ../ECommerceOS.AppHost
```

Standalone:

```bash
dotnet run --project ECommerceOS.CatalogService.WebApi
```

Standalone execution requires PostgreSQL, Kafka, Schema Registry, Redis, blob storage, and any downstream service dependencies to be available.

## Test

```bash
dotnet test ECommerceOS.CatalogService.slnx
```
