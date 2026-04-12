using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.UnitTests.Helpers;

/// <summary>
/// Factory for creating test instances of AppDbContext using InMemory database.
/// </summary>
public static class TestDbContextFactory
{
    /// <summary>
    /// Creates a new AppDbContext with InMemory database for testing.
    /// </summary>
    /// <param name="dbName">Optional database name for isolation between tests.</param>
    /// <returns>A new AppDbContext instance.</returns>
    public static AppDbContext CreateTestDbContext(string dbName = "TestDb")
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName + Guid.NewGuid())
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}

