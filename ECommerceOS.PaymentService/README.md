# ECommerceOS.PaymentService

`ECommerceOS.PaymentService` owns payment orchestration and Stripe integration. It exposes payment and transaction endpoints, processes payment-related events, serves checkout gRPC requests, and handles Stripe webhooks.

## Solution layout

- `ECommerceOS.PaymentService.Application` contains payment and transaction use cases
- `ECommerceOS.PaymentService.Domain` contains payment and transaction domain logic
- `ECommerceOS.PaymentService.Domain.Tests` contains domain tests
- `ECommerceOS.PaymentService.Infrastructure` contains persistence, messaging, Stripe integrations, caching, background jobs, and state machines
- `ECommerceOS.PaymentService.Presentation` contains HTTP groups, gRPC services, webhook endpoints, and exception handling
- `ECommerceOS.PaymentService.WebApi` is the service host

## HTTP and gRPC surface

HTTP route groups:

- `/payment`
- `/transaction`
- `/stripe`

The HTTP surface includes:

- initialize payment
- get payment by id
- cancel payment
- create transaction session
- get transaction by id
- cancel transaction
- get transaction status
- process Stripe webhook events

gRPC surface:

- `GetCheckoutSession` via the `Checkout` contract in `ECommerceOS.Shared/Contracts/ProtoBuf/Checkout.proto`

## Infrastructure dependencies

- PostgreSQL database: `paymentdb`
- Kafka producer for `payment-event`
- Kafka consumers for `order-event`, `catalog-event`, and `identity-event`
- Schema Registry for Avro serialization
- Redis-backed caching when provisioned by Aspire
- Quartz for scheduled outbox publishing
- Stripe client, webhook processing, and refund/session services
- JWT authentication configuration

## Configuration

Development configuration lives in `ECommerceOS.PaymentService.WebApi/appsettings.Development.json`.

Key sections:

- `ConnectionStrings:paymentdb`
- `ConnectionStrings:kafka`
- `ConnectionStrings:schemaRegistry`
- `JwtOptions`
- `StripeWebhookOptions`
- `SmtpClientOptions`

## Run locally

Preferred:

```bash
dotnet run --project ../ECommerceOS.AppHost
```

Standalone:

```bash
dotnet run --project ECommerceOS.PaymentService.WebApi
```

Standalone execution requires PostgreSQL, Kafka, Schema Registry, Redis if you want cache-backed behavior, plus valid Stripe settings for end-to-end payment flows.

## Test

```bash
dotnet test ECommerceOS.PaymentService.slnx
```
