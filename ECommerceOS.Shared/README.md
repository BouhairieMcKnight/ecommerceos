# ECommerceOS.Shared

`ECommerceOS.Shared` contains the contracts and reusable primitives shared across the services.

## Contents

- `Auth` for common authentication helpers
- `Contracts` for integration-event contracts and service interfaces
- `Contracts/ProtoBuf` for gRPC definitions shared between services
- `DTOs` for shared request and response shapes
- `Entity` and `ValueObjects` for reusable domain primitives
- `Result` for shared result handling

## gRPC contracts

The shared protobuf definitions currently include:

- `Checkout.proto` for catalog-to-payment checkout session requests
- `Reserve.proto` for order-to-catalog inventory reservation requests

## Why this project matters

This project is the contract boundary between services. Keeping message contracts, protobuf definitions, and shared value objects here reduces duplication and keeps HTTP, gRPC, and Kafka integrations aligned across the solution.
