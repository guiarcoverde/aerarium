# CLAUDE.md - Aerarium

## Overview
Aerarium is a personal financial manager web application built by the Animus team.

## Tech Stack
- .NET 10, ASP.NET Core Minimal APIs
- Entity Framework Core 10 with PostgreSQL
- ASP.NET Identity with EF Core for user management
- Mediator for CQRS (source-generated, https://github.com/martinothamar/Mediator)
- FluentValidation for request validation
- Scalar for OpenAPI documentation
- xUnit + FluentAssertions for testing

## Project Structure
- `src/Api/` - Endpoints, middleware, DI configuration
- `src/Application/` - Commands, queries, handlers, validators
- `src/Domain/` - Entities, value objects, enums, domain events
- `src/Infrastructure/` - EF Core, external services, repositories
- `tests/UnitTests/` - Domain and application layer tests
- `tests/IntegrationTests/` - API and database tests

## Commands
- Build: `dotnet build`
- Test: `dotnet test`
- Run API: `dotnet run --project src/Api`
- Add Migration: `dotnet ef migrations add <Name> -p src/Infrastructure -s src/Api`
- Update Database: `dotnet ef database update -p src/Infrastructure -s src/Api`
- Format: `dotnet format`

## Architecture Rules
- Domain layer has ZERO external dependencies
- Application layer defines interfaces, Infrastructure implements them
- All database access goes through EF Core DbContext (no repository pattern)
- Use Mediator for all command/query handling
- API layer is thin - endpoint definitions only

## Authentication & Authorization
- ASP.NET Identity with EF Core for user management
- JWT Bearer tokens for API authentication
- Credentials stored in AWS SSM Parameter Store (not appsettings)
- Authorization via endpoint-level policies, not controller attributes

## Configuration
- Secrets come from AWS SSM Parameter Store in production
- Local development uses `appsettings.Development.json` (never commit secrets)
- Connection string key: `ConnectionStrings:Default`
- Never suggest hardcoded secrets or appsettings.json for sensitive values

## Error Handling
- Handlers return `Result<T>` - never throw for business logic
- Global exception middleware in Api layer handles unexpected exceptions
- Use ProblemDetails (RFC 7807) format for all error responses
- Map Result errors to appropriate HTTP status codes in endpoints

## Endpoint Conventions
- One file per feature/entity: `UsersEndpoints.cs`, `TransactionsEndpoints.cs`
- Group endpoints with `RouteGroupBuilder`
- Extension method pattern: `app.MapUserEndpoints()`
- No logic in endpoints - dispatch to Mediator immediately

## EF Core Conventions
- No lazy loading - use explicit `Include()` when needed
- Configurations via `IEntityTypeConfiguration<T>`, never Data Annotations
- DbContext is in Infrastructure layer
- Never expose `IQueryable` outside of handlers

## HTTP Clients
- Always use `IHttpClientFactory` - never instantiate `HttpClient` directly
- Named clients configured in DI (`Program.cs`)
- Typed clients preferred for external service integrations
- Retry/timeout policies via Polly configured on registration

### Example pattern
- Define typed client in Infrastructure layer
- Register with `builder.Services.AddHttpClient<IMyServiceClient, MyServiceClient>()`
- Inject typed client directly into handlers or services

## Code Conventions

### Naming
- Commands: `Create[Entity]Command`, `Update[Entity]Command`
- Queries: `Get[Entity]Query`, `List[Entities]Query`
- Handlers: `[Command/Query]Handler`
- DTOs: `[Entity]Dto`, `Create[Entity]Request`

### Patterns We Use
- Primary constructors for DI
- Records for DTOs and commands
- `Result<T>` pattern for error handling (no exceptions for flow control)
- File-scoped namespaces
- Always pass `CancellationToken` to async methods

### Patterns We DON'T Use (Never Suggest)
- Repository pattern (use EF Core directly)
- AutoMapper (write explicit mappings)
- Exceptions for business logic errors
- Stored procedures
- `new HttpClient()` (always use `IHttpClientFactory`)

## Validation
- All request validation in FluentValidation validators
- Validators auto-registered via assembly scanning
- Validation runs in Mediator pipeline behavior

## Testing
- Unit tests: Domain logic and handlers
- Integration tests: Full API endpoint testing with WebApplicationFactory
- Use FluentAssertions for readable assertions
- Test naming: `[Method]_[Scenario]_[ExpectedResult]`

## Git Workflow
- Branch naming: `feature/`, `bugfix/`, `hotfix/`
- Commit format: `type: description` (feat, fix, refactor, test, docs)
- Always create a branch before changes
- Run tests before committing

## Architecture Decisions
- **No Repository pattern**: EF Core DbContext is the abstraction - adding a repository on top adds indirection with no benefit
- **No AutoMapper**: explicit mapping keeps code readable, refactorable, and avoids magic
- **Source-generated Mediator**: prefer compile-time over runtime reflection (martinothamar/Mediator, not MediatR)
- **No exceptions for flow control**: `Result<T>` makes error paths explicit and testable
- **No Data Annotations**: all EF Core config via `IEntityTypeConfiguration<T>` for separation of concerns

## When in Doubt
- Prefer boring, explicit code over clever abstractions
- If a pattern isn't listed here, ask before implementing
- Smaller PRs preferred - one feature per branch
