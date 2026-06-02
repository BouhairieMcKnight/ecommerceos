# ECommerceOS.AuthService

`ECommerceOS.AuthService` owns identity and authentication workflows for the platform. It is responsible for account creation, login, refresh-token handling, logout, email verification, and identity integration events.

## Solution layout

- `ECommerceOS.AuthService.Application` contains identity use cases and request handlers
- `ECommerceOS.AuthService.Domain` contains identity domain types
- `ECommerceOS.AuthService.Infrastructure` contains persistence, messaging, caching, encryption, email, Quartz jobs, and security configuration
- `ECommerceOS.AuthService.Presentation` contains HTTP endpoint mappings and HTTP helpers
- `ECommerceOS.AuthService.WebApi` is the service host
- `ECommerceOS.AuthService.Tests` contains the service test project

## API surface

The service exposes its HTTP endpoints under `/auth`:

- register
- login
- refresh
- logout
- verify-email
- delete account

When running through the Aspire gateway, auth traffic is registered in `ECommerceOS.AppHost/AppHost.cs`.

## Infrastructure dependencies

- PostgreSQL database: `identitydb`
- Kafka producer for `identity-event`
- Schema Registry for Avro serialization
- Redis for distributed caching
- Quartz for scheduled outbox publishing
- SMTP for email delivery
- Google OAuth settings for external authentication flows

## Configuration

Development configuration lives in `ECommerceOS.AuthService.WebApi/appsettings.Development.json`.

Key sections:

- `ConnectionStrings:identitydb`
- `ConnectionStrings:kafka`
- `ConnectionStrings:schemaRegistry`
- `ConnectionStrings:cache`
- `JwtOptions`
- `KeyOptions`
- `Authentication:Google`
- `SmtpClientOptions`

## Run locally

Preferred:

```bash
dotnet run --project ../ECommerceOS.AppHost
```

Standalone:

```bash
dotnet run --project ECommerceOS.AuthService.WebApi
```

For standalone execution, PostgreSQL, Kafka, Schema Registry, Redis, and any external credentials must already be available.

## Test

```bash
dotnet test ECommerceOS.AuthService.slnx
```
