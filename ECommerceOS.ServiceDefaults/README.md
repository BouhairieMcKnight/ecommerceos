# ECommerceOS.ServiceDefaults

`ECommerceOS.ServiceDefaults` holds the common Aspire service configuration shared by the service hosts.

## What it provides

- OpenTelemetry logging, tracing, and metrics
- ASP.NET Core and `HttpClient` instrumentation
- OTLP exporter wiring when `OTEL_EXPORTER_OTLP_ENDPOINT` is configured
- default health checks
- service discovery
- standard HTTP resilience handlers

## Default endpoints

In Development, services that call `MapDefaultEndpoints()` expose:

- `/health`
- `/alive`

These endpoints are intentionally limited to Development by the shared extension.

## Why it exists

Without this project, each service would need to duplicate the same resilience, telemetry, health-check, and discovery setup. Keeping it here makes the distributed-system defaults consistent across every host.
