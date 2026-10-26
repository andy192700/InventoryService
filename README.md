# The Inventory Service

This repository contains a basic inventory system, built in .NET and dockerized. It intentionally prioritizes maintainability, testability and developer experience over feature completeness.

It is primarily a personal project used to explore application architecture, testing strategies, deployment tooling and database design outside of a production environment.

The goal of this repository is to provide a realistic service that can be continuously evolved as new concepts, patterns and technologies are explored.

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

Its database schema is implemented in SQL Server and is [dockerized](./docker/) with a Flyway container for managing migrations.

There is also a [Bruno](./bruno/) collection for sending requests to the API.

## Application Architecture

The application follows a vertical slice architecture, where each feature owns its requests, handlers, validation and API contracts.

Key design decisions include:
- ASP.NET Minimal APIs for endpoint definitions
- MediatR for request handling and pipeline composition
- Dapper for lightweight data access
- SQL Server as the backing data store
- Middleware for cross-cutting concerns such as request logging
- Flyway for database versioning and migration management
- Docker Compose for local environment orchestration
- Lightweight basic authorization/authentication

### Repository Structure
/src
- InventoryService
- InventoryService.UnitTests
- InventoryService.IntegrationTests

/docker
- Docker Compose configuration
- SQL Server container
- Flyway migrations

/bruno
- API collections and environments

### Unit Testing

Unit tests focus on business logic, request validation and application behaviour. External dependencies are mocked using Moq, while AutoFixture is used to generate representative test data.

### Integration Testing

Integration tests execute requests against a fully hosted application instance using a real HTTP pipeline.

A SQL Server Docker container is started for the duration of the test run and migrated using Flyway. This allows integration tests to execute against a real database rather than relying on mocks or in-memory implementations.

## Prerequisites

- .NET 10
- PowerShell 7.6
- Bruno
- Docker
- PSDoFramework

### What is the PSDoFramework and what is a dofile.ps1?

PSDoFramework is a PowerShell module that I developed to provide developer tooling and task orchestration. In this repository it is used in place of a traditional Makefile to automate common development workflows.

To install the module, run the following PowerShell command

```PowerShell
install-Module -Name PSDoFramework
```

- GitHub repository: https://github.com/andy192700/psdoframework
- PSGallery: https://www.powershellgallery.com/packages/PSDoFramework

## Running locally 

Assuming all prerequisites, run the following PowerShell command to stand up the containers (app, db and flyway):

```PowerShell
doing exec -target DockerUp
```

With the containers running, to target the API via Bruno:
- Open Bruno
- Within Bruno open the [collection](./bruno/InventoryAPI/) and ensure the environment `local_env` is selected.

To remove the docker containers, images and volumes, run:

```PowerShell
doing exec -target DockerDown
```

To run the tests via the commandline, run:

```PowerShell
doing exec -target DotnetTest
```

To create a new API login, run the following:
```PowerShell
doing exec -target CreateCredential -username MyUser -password MyPassword
```