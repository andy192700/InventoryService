# The Inventory Service

This repository contains a sample inventory management system built with .NET and Docker. It intentionally prioritizes maintainability, testability, observability and developer experience over feature completeness.

The project serves as a platform for exploring modern application architecture and engineering practices, including feature-driven design, CQRS, messaging patterns, automated testing, database evolution and containerised development workflows.

The goal is to provide a realistic service that can be continuously evolved as new concepts, patterns and technologies are explored.

## What is it?

### Technology Stack

- .NET 10
- ASP.NET Minimal APIs
- FluentValidation
- FluentResults
- FluentAssertions
- MediatR
- Dapper
- SQL Server
- Flyway
- AWS SNS
- AWS SQS
- LocalStack
- Docker / Docker Compose
- xUnit
- Moq
- AutoFixture
- Bruno
- PowerShell

The Inventory Service is a .NET Minimal API that exposes a RESTful interface for managing products, customers and purchase orders.

Current functionality includes:

- Create products, customers and purchase orders
- Retrieve products, customers and purchase orders
- List products, customers and purchase orders
- List supported countries and currencies
- Publish and consume domain events through a messaging infrastructure

Its database schema is implemented in SQL Server and is dockerized with a Flyway container responsible for managing database versioning and migrations.

The solution also contains a reusable **Messaging** library that demonstrates asynchronous communication using Amazon SNS and Amazon SQS. The messaging infrastructure can be executed locally through LocalStack, allowing event-driven workflows to be developed and tested without requiring access to AWS.

There is also a [Bruno](./bruno/) collection for sending requests to the API.

## Application Architecture

The application follows a **feature-driven vertical slice architecture**, where each business capability owns its endpoints, requests, handlers, validation, contracts and data access concerns.

Features are organised around business behaviour rather than technical layers, helping to minimise coupling and allowing functionality to be developed, tested and maintained in isolation.

The service also employs a **CQRS (Command Query Responsibility Segregation)** style architecture using MediatR. Commands and queries are modelled as separate request types, allowing write and read operations to evolve independently while maintaining a simple and consistent request-processing pipeline.

### Key Design Decisions

- Feature-driven vertical slice architecture
- CQRS using MediatR commands and queries
- ASP.NET Minimal APIs for endpoint definitions
- FluentValidation for request validation
- Dapper for lightweight data access
- SQL Server as the primary data store
- Middleware for cross-cutting concerns such as logging, exception handling and Problem Details generation
- Flyway for database versioning and migration management
- Docker Compose for local environment orchestration
- Lightweight role-based authentication and authorization
- Event-driven messaging using SNS and SQS
- LocalStack for local AWS service emulation

### Messaging Architecture

The solution contains a dedicated **Messaging** library which provides a producer-consumer implementation based on **Amazon SNS** and **Amazon SQS**.

Messages are published to SNS topics and distributed to subscribed SQS queues, allowing producers and consumers to remain loosely coupled.

This approach provides:

- Asynchronous processing
- Improved scalability
- Failure isolation between services
- Reliable message delivery through queue-based consumption
- Independent deployment and evolution of producers and consumers

For local development and integration testing, SNS and SQS are hosted through LocalStack, providing a lightweight AWS-compatible environment that can be executed entirely within Docker.

### Repository Structure

```text
/src
├── InventoryService
├── InventoryService.Messaging
├── InventoryService.UnitTests
└── InventoryService.IntegrationTests

/docker
├── Docker Compose configuration
├── SQL Server container
├── Flyway migrations
└── LocalStack container

/bruno
└── API collections and environments
```

### Unit Testing

Unit tests focus on business logic, request validation and application behaviour. External dependencies are mocked using Moq while AutoFixture is used to generate representative test data.

The feature-driven architecture allows handlers, validators and supporting services to be tested independently with minimal setup.

### Integration Testing

Integration tests execute requests against a fully hosted application instance using the real ASP.NET HTTP pipeline.

A SQL Server Docker container is started for the duration of the test run and migrated using Flyway. Where messaging workflows are involved, LocalStack can be used to provide SNS and SQS infrastructure, allowing end-to-end testing of asynchronous processing scenarios.

This approach enables integration tests to execute againstc infrastructure rather than relying on mocks or in-memory implementations.

## Prerequisites

- .NET 10
- PowerShell 7.6
- Bruno
- Docker
- PSDoFramework

### What is PSDoFramework and what is a dofile.ps1?

PSDoFramework is a PowerShell module that I developed to provide developer tooling and task orchestration. Within this repository it serves a similar purpose to a Makefile, providing a consistent mechanism for executing common development tasks.

To install the module:

```powershell
Install-Module -Name PSDoFramework
```

- GitHub Repository: https://github.com/andy192700/psdoframework
- PowerShell Gallery: https://www.powershellgallery.com/packages/PSDoFramework

## Running Locally

Assuming all prerequisites are installed, run the following command to start the local development environment:

```powershell
doing exec -target DockerUp
```

This will start:

- Inventory Service
- SQL Server
- Flyway
- LocalStack

To interact with the API using Bruno:

1. Open Bruno.
2. Open the collection located at `./bruno/InventoryAPI`.
3. Select the `local_env` environment.

To tear down the local environment, including containers, images and volumes:

```powershell
doing exec -target DockerDown
```

To execute all tests:

```powershell
doing exec -target DotnetTest
```

To create a new API login:

```powershell
doing exec -target CreateCredential -username MyUser -password MyPassword
```

To add a permission to an API login (`Read` or `Write`):

```powershell
doing exec -target AddPermission -username MyUser -permission Write
```