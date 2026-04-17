using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.UnitTests.Helpers;

/// <summary>
/// Factory for creating test instances of AppDbContext using PostgreSQL database.
/// </summary>
public static class TestDbContextFactory
{
    private static readonly string ConnectionString = 
        "Host=localhost;Port=5433;Database=monavixxhub_test;Username=postgres;Password=postgres";

    /// <summary>
    /// Creates a new AppDbContext with PostgreSQL database for testing.
    /// Uses a unique database for each test to ensure isolation.
    /// </summary>
    /// <param name="dbName">Optional database name for isolation between tests.</param>
    /// <returns>A new AppDbContext instance.</returns>
    public static AppDbContext CreateTestDbContext(string dbName = "TestDb")
    {
        var uniqueDbName = dbName + "_" + Guid.NewGuid().ToString("N");
        var testConnectionString = ConnectionString.Replace("monavixxhub_test", uniqueDbName);
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(testConnectionString)
            .Options;

        var context = new AppDbContext(options);
        
        try
        {
            // Create database and apply all migrations
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            // If migration fails, ensure database is deleted and try again
            try
            {
                context.Database.EnsureDeleted();
                context.Database.Migrate();
            }
            catch
            {
                throw new InvalidOperationException(
                    $"Failed to initialize test database '{uniqueDbName}'. " +
                    $"Make sure PostgreSQL is running on localhost:5433 and migrations are available.", ex);
            }
        }
        
        return context;
    }
    
    /// <summary>
    /// Cleans up the test database after test completion.
    /// </summary>
    /// <param name="context">The AppDbContext instance to clean up.</param>
    public static void CleanupTestDb(AppDbContext context)
    {
        try
        {
            context.Database.EnsureDeleted();
        }
        catch
        {
            // Ignore errors during cleanup
        }
        context.Dispose();
    }
}

