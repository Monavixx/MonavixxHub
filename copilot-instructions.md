# Instructions for MonavixxHub.Api

## Project Overview

MonavixxHub.Api is a .NET 10 REST API project built with C# that provides functionality for managing flashcards, user authentication, image handling, and study sessions.

### Tech Stack
- **Framework**: ASP.NET Core (.NET 10)
- **Database**: Entity Framework Core with SQL Server/PostgreSQL
- **Authentication**: JWT-based authentication with role-based authorization
- **Additional Features**: Image processing, Email confirmation, Study sessions
- **Validation**: DataAnnotations or FluentValidation for request validation

## Project Structure

```
MonavixxHub.Api/
├── Features/                 # Feature-based folder structure
│   ├── Admin/               # Admin management features
│   ├── Auth/                # Authentication and registration
│   ├── Flashcards/          # Flashcard CRUD operations
│   ├── FlashcardsStudy/      # Study session logic
│   └── Images/              # Image upload and processing
├── Infrastructure/          # Database context and EF Core setup
├── Common/                  # Shared utilities
│   ├── Authorization/       # Custom authorization handlers
│   ├── Exceptions/          # Global exception handling
│   ├── Policies.cs          # Authorization policies
│   └── Services/            # Common services
├── Migrations/              # EF Core database migrations
└── Properties/              # Launch settings and configuration
```

## Key Files & Their Purposes

| File | Purpose |
|------|---------|
| `Program.cs` | Application startup, DI configuration, middleware setup |
| `AppDbContext.cs` | Entity Framework Core database context |
| `appsettings.json` | Configuration settings (database, JWT, etc.) |
| `Policies.cs` | Authorization policy definitions |
| `GlobalExceptionHandler.cs` | Centralized exception handling |
| `GlobalAuthorizationHandler.cs` | Custom authorization logic |

## Coding Standards & Conventions

### General Rules
1. **Naming**: Follow PascalCase for classes, interfaces, and public methods
2. **File Organization**: One class per file (unless internal/helper classes)
3. **Namespace**: Match folder structure (e.g., `MonavixxHub.Api.Features.Auth`)
4. **Async/Await**: Use async patterns for I/O operations (database, HTTP calls)
5. **Documentation**: Use XML comments for public methods and classes
6. **Collections**: Use collection expressions (e.g. `var arr = [1, 2, 3]`)

### Feature Development
- Use the feature folder pattern: Each feature should have its own directory
- Include: Controllers, Services, DTOs, Models, and Exceptions within each feature folder
- Follow vertical slice architecture principles

### Database & EF Core
1. Create migrations for any database changes: `dotnet ef migrations add MigrationName`
2. Always include descriptive migration names
3. Use `DbSet<T>` properties for all entities in `AppDbContext`
4. Include proper shadow properties and relationships

### API Endpoints
1. Use RESTful conventions
2. Include proper HTTP status codes
3. Return DTOs (Data Transfer Objects), not entities
4. Document endpoints with XML comments and OpenAPI annotations

### Authentication & Authorization
1. Use JWT tokens for API authentication
2. Apply `[Authorize]` attributes where needed
3. Use custom authorization policies from `Policies.cs`
4. Check roles using `[Authorize(Roles = "Admin")]` when applicable

### Exception Handling
- Inherit from `AppBaseException` for custom exceptions
- Let `GlobalExceptionHandler` middleware handle all exceptions
- Never catch and suppress exceptions without logging

## Common Tasks

### Adding a New Feature
1. Create a new folder in `Features/`
2. Add Controller, Service, DTOs, and Models as needed
3. Register services in `Program.cs` dependency injection
4. Create endpoints following RESTful conventions
5. Add authorization attributes if needed

### Database Changes
1. Modify entity models
2. Update `AppDbContext.cs` if adding new entities
3. Run: `dotnet ef migrations add [MigrationName]`
4. Review generated migration
5. Run: `dotnet ef database update`

### Adding Authorization
1. Define policy in `Policies.cs` if not exists
2. Add policy to `Program.cs` configuration
3. Apply `[Authorize(Policy = Policies.PolicyName)]` to endpoints

## Testing

- Unit tests located in `MonavixxHub.Api.UnitTests/`
- Use xUnit framework for testing
- Follow Arrange-Act-Assert pattern
- Mock external dependencies (database, HTTP calls)
- Test file naming: `[FeatureName]Tests.cs`
- Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken to allow test cancellation to be more responsive.

## Running the Project

```bash
# Restore NuGet packages
dotnet restore

# Apply migrations
dotnet ef database update

# Build the project
dotnet build

# Run the project
dotnet run

# Run tests
dotnet test

# Run in watch mode (development)
dotnet watch run
```

## Environment Configuration

- **Development**: `appsettings.Development.json` - Local development settings
- **Production**: `appsettings.json` - Default/production settings
- **Key Settings**:
  - Database connection string
  - JWT secret and expiration
  - Allowed origins for CORS
  - Image upload limits

## Important Considerations

1. **Security**: 
   - Always validate user input
   - Use parameterized queries (EF Core handles this)
   - Don't expose sensitive information in error messages
   - Use HTTPS in production

2. **Performance**:
   - Use `async/await` for I/O operations
   - Implement proper indexing on frequently queried columns
   - Consider pagination for list endpoints
   - Use projection (`.Select()`) to avoid loading unnecessary data

3. **Logging**:
   - Use ILogger for logging
   - Log important operations and errors
   - Check `logs/` folder for application logs

## When in Doubt

1. Look at existing features for patterns and examples
2. Check `Program.cs` for middleware and DI registration
3. Review `GlobalExceptionHandler.cs` for error handling
4. Consult `Policies.cs` for authorization requirements
5. Check models and their configurations for schema understanding

---

Last updated: April 2026

