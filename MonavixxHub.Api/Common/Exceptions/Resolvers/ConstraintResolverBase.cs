using System.Collections.Concurrent;
using System.Collections.Frozen;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace MonavixxHub.Api.Common.Exceptions.Resolvers;

public abstract class ConstraintResolverBase<TSelf>(ILogger<TSelf> logger)
{
    private readonly ConcurrentDictionary<Type, Lazy<FrozenDictionary<string, string>>> _cache = new();
    protected abstract FrozenDictionary<string, string> BuildMap(DbContext dbContext);

    protected FrozenDictionary<string, string> GetOrBuildMap(DbContext dbContext)
        => _cache.GetOrAdd(dbContext.GetType(),
            _ => new Lazy<FrozenDictionary<string, string>>(() => BuildMap(dbContext),
                LazyThreadSafetyMode.ExecutionAndPublication)).Value;

    public string? Resolve(PostgresException postgresException, DbContext dbContext)
    {
        var map = GetOrBuildMap(dbContext);
        if (map.TryGetValue(postgresException.ConstraintName!, out var response))
            return response;
        logger.LogWarning("{ResolverName}: constraint {ConstraintName} not found in EF Core model.",
            typeof(TSelf).Name, postgresException.ConstraintName);
        return null;
    }
}