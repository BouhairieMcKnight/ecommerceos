# ECommerceOS.AppHost

`ECommerceOS.AppHost` is the local orchestration entrypoint for the distributed system. It provisions infrastructure, starts every service, configures service discovery, and exposes the YARP gateway used during local development.

## Responsibilities

- Starts the four service hosts: auth, catalog, order, and payment
- Provisions PostgreSQL databases for each bounded context
- Provisions Redis, Kafka, Kafka UI, Schema Registry, and Azure Storage emulator
- Creates the integration-event topics used by the services
- Exposes a single YARP gateway endpoint over the service mesh

## Provisioned resources

- Redis: `cache`
- Kafka broker plus Kafka UI
- Schema Registry
- PostgreSQL server with:
  - `identitydb`
  - `catalogdb`
  - `orderdb`
  - `paymentdb`
- PgAdmin on host port `5050`
- Azure Storage emulator plus blob container connection named `blobs`

## Event topics

The AppHost creates these Kafka topics when the broker becomes ready:

- `catalog-event`
- `payment-event`
- `order-event`
- `identity-event`

## Gateway

The gateway is configured in `AppHost.cs` and currently exposes:

- Auth traffic under `/api/auth/{**catch-all}`
- Order traffic under `/api/order/{**catch-all}`
- Catalog traffic under `/api/catalog/{**catch-all}`
- Payment traffic under `/api/payment/{**catch-all}`
- Stripe webhook forwarding through `/webhook`

The HTTPS gateway endpoint is bound to `https://localhost:8001`.

## Run locally

```bash
dotnet run --project ECommerceOS.AppHost
```

This is the preferred way to run the platform locally because it supplies connection strings and service references automatically.

## Notes

- The launch profile also starts the Aspire dashboard and resource service endpoints dynamically.
- Service-to-service dependencies are declared here, including the gRPC relationships between catalog/payment and order/catalog.
