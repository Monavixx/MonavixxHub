# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a .NET 10.0 web API project implementing a flashcard learning application with authentication, role-based authorization, and background job processing.

## Architecture

- **MonavixxHub.Api**: Main API project with Clean Architecture structure
  - **Common**: Shared utilities, base classes, and common services
  - **Features**: Feature modules (Auth, Flashcards, FlashcardsStudy, Images, Admin)
  - **Infrastructure**: Database context, migrations, and infrastructure services
- **MonavixxHub.Api.UnitTests**: Unit test project

## Key Technologies

- ASP.NET Core 10.0 with minimal APIs
- Entity Framework Core 10.0 with PostgreSQL (Npgsql)
- Hangfire 1.8.23 for background job processing
- Serilog for structured logging
- FluentValidation for request validation
- JWT Bearer authentication
- Rate limiting with sliding window policies
- AutoMapper (implied by mapping profiles in Features)

## Development Commands

### Build and Run
```bash
dotnet build
dotnet run --project MonavixxHub.Api
```

### Run Tests
Don't run tests ever.

### Background Jobs (Hangfire)
- Jobs are configured in `Program.cs`
- Uses PostgreSQL storage via `Hangfire.PostgreSql`
- Hangfire dashboard available at `/hangfire` in development

## Restrictions

### Command Line Tools
- **Do not use bash, cmd, or shell commands** for file searching, reading, or content analysis (find, grep, cat, etc.)
- Use the IDE's built-in search functionality or the dedicated search tools provided by this system instead
- This ensures consistent and safe file operations within the development environment

## Important Patterns

### Dependency Injection
- Singleton services for stateless operations
- Scoped services for request-specific operations
- Authorization handlers registered as scoped

### Error Handling
- Global exception handler configured via `AddExceptionHandler<GlobalExceptionHandler>()`
- Problem details enabled for consistent error responses
- Custom exceptions with resolver pattern in `Common.Exceptions`

### Security
- JWT authentication with issuer/audience validation
- Role-based authorization with requirements
- Rate limiting on authentication endpoints
- HTTPS redirection enforced

### Logging
- Structured logging with Serilog expressions
- Console and file sinks (daily rolling)
- Request context enrichment (RequestId, UserId, etc.)

## Configuration

Key configuration sections:
- `StorageOptions`: File storage settings
- `EmailOptions`: Email service settings
- `RateLimitingOptions`: Rate limit policies
- `ConnectionStrings`: Database and Hangfire connections
- `Jwt`: Token validation settings

## Testing

- Unit tests in `MonavixxHub.Api.UnitTests`
- Uses xUnit
- Database tests uses postgreSQL test database

## File Structure

- `Features/` - Feature modules with controllers, services, validators
- `Common/` - Shared utilities, exceptions, options
- `Infrastructure/` - Database context, repositories