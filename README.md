# Order Processing System

Microservices-based order processing system built with ASP.NET Core, Kafka messaging, and gRPC communication.

[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-512BD4?style=flat&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/apps/aspnet)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?style=flat&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Apache Kafka](https://img.shields.io/badge/Kafka-231F20?style=flat&logo=apachekafka&logoColor=white)](https://kafka.apache.org/)
[![gRPC](https://img.shields.io/badge/gRPC-244c5a?style=flat&logo=google&logoColor=white)](https://grpc.io/)
[![Docker](https://img.shields.io/badge/Docker-2496ED?style=flat&logo=docker&logoColor=white)](https://www.docker.com/)

## Overview

A distributed order processing service built with ASP.NET Core implementing Clean Architecture principles. The system handles order lifecycle management including creation, approval, packing, and delivery stages with full event sourcing through Kafka.

## Architecture

```
                                 +------------------+
                                 |   REST Gateway   |
                                 |   (HTTP/JSON)    |
                                 +--------+---------+
                                          |
                                          | gRPC
                                          v
+------------------+            +------------------+            +------------------+
|                  |   Kafka    |                  |            |                  |
|  External        +----------->+  Order Service   +----------->+   PostgreSQL     |
|  Services        |   Events   |  (gRPC API)      |            |   Database       |
|                  |<-----------+                  |            |                  |
+------------------+            +------------------+            +------------------+
```

| Layer          | Technology                          |
|----------------|-------------------------------------|
| Runtime        | ASP.NET Core 9                      |
| API Gateway    | ASP.NET Core REST API               |
| Internal API   | gRPC with Protobuf                  |
| Messaging      | Apache Kafka                        |
| Database       | PostgreSQL                          |
| Migrations     | FluentMigrator                      |
| HTTP Client    | Refit                               |
| Containerization | Docker, Docker Compose            |

## Project Structure

```
src/
├── Api/                          # Main application host
├── Gateway/                      # REST API Gateway
├── Gateway.Host/                 # Gateway application host
├── Application/
│   ├── Core/                     # Business logic, services
│   ├── Abstractions/             # Interfaces, ports
│   ├── Contracts/                # DTOs, service contracts
│   └── Domain/                   # Entities, enums, value objects
├── Infrastructure/
│   ├── Configuration/            # External configuration service client
│   ├── Kafka/                    # Kafka producer/consumer implementation
│   ├── Kafka.Contracts/          # Protobuf message definitions
│   └── Postgres/                 # Repository implementations, migrations
└── Presentation/
    ├── Api/                      # gRPC service implementations
    └── Protos/                   # gRPC service definitions
```

## Features

- Order Management
  - Create orders with multiple items
  - Add/remove items from orders
  - View order history with full audit trail

- Order Processing Pipeline
  - Order approval workflow
  - Packing stage management
  - Delivery tracking

- Event-Driven Architecture
  - Asynchronous event processing via Kafka
  - Event sourcing for order state changes

- External Configuration
  - Dynamic configuration updates
  - Centralized configuration service integration

## Getting Started

### Prerequisites

- .NET 9 SDK
- Docker and Docker Compose
- PostgreSQL (or use Docker)
- Apache Kafka (or use Docker)

### Configuration

Application settings are configured in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Database=orders;Username=postgres;Password=postgres"
  },
  "Kafka": {
    "BootstrapServers": "localhost:9092"
  }
}
```

## API Endpoints

### Gateway REST API

| Method | Endpoint                     | Description              |
|--------|------------------------------|--------------------------|
| POST   | /api/products                | Create products          |
| GET    | /api/products                | List products            |
| POST   | /api/orders                  | Create order             |
| GET    | /api/orders/{id}             | Get order by ID          |
| POST   | /api/orders/{id}/items       | Add items to order       |
| GET    | /api/orders/{id}/history     | Get order history        |
| POST   | /api/processing/approve      | Approve order            |
| POST   | /api/processing/packing/start| Start packing            |
| POST   | /api/processing/packing/finish| Finish packing          |
| POST   | /api/processing/delivery/start| Start delivery          |
| POST   | /api/processing/delivery/finish| Complete delivery      |

## Order States

| State       | Description                              |
|-------------|------------------------------------------|
| Created     | Order created, awaiting approval         |
| Approved    | Order approved, ready for packing        |
| Packing     | Order is being packed                    |
| Packed      | Order packed, ready for delivery         |
| Delivering  | Order is in delivery                     |
| Delivered   | Order successfully delivered             |
| Cancelled   | Order cancelled                          |

## Development

### Building

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

### Code Structure Conventions

- **Application.Abstractions** - Contains interfaces that define ports (repository interfaces, messaging interfaces)
- **Application.Contracts** - Contains DTOs and service interfaces exposed to other layers
- **Application.Core** - Contains business logic implementations
- **Domain** - Contains domain entities, enums, and filters
- **Infrastructure.*** - Contains implementations of abstractions (adapters)
- **Presentation.*** - Contains API layer (controllers, gRPC services)

## License

MIT
