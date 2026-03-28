using System.Collections.Concurrent;
using System.Collections.Frozen;
using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MonavixxHub.Api.Common.Exceptions;

/// <summary>
/// Resolves <see cref="UniqueConstraintException"/> into a user-friendly message
/// describing which database column(s) caused the violation.
/// Thread-safe.
/// Note: doesn't work with Sqlite as it doesn't populate ConstraintName.
/// </summary>
public class UniqueConstraintResolver (ILogger<UniqueConstraintException> logger)
{
    private readonly ConcurrentDictionary<Type, Lazy<FrozenDictionary<string,string>>>
        _cacheUniqueConstraints = new();

    
    private FrozenDictionary<string, string> GetOrBuildUniqueConstraintMap(DbContext dbContext)
        // LazyThreadSafetyMode.ExecutionAndPublication guarantees that BuildUniqueConstraintMap
        // will be called once.
        => _cacheUniqueConstraints.GetOrAdd(dbContext.GetType(),
            _ => new Lazy<FrozenDictionary<string, string>>(() => BuildUniqueConstraintMap(dbContext),
                LazyThreadSafetyMode.ExecutionAndPublication)).Value;

    private FrozenDictionary<string, string> BuildUniqueConstraintMap(DbContext dbContext)
    {
        var map = new Dictionary<string, string>();
    
        foreach (var entityType in dbContext.Model.GetEntityTypes())
        {
            var indexes = entityType.GetIndexes()
                .Where(i => i.IsUnique);
            foreach (var index in indexes)
            {
                map[index.GetDatabaseName()!] = GetResponseByIndex(index);
            }
        }
    
        return map.ToFrozenDictionary();
    }

    private string GetResponseByIndex(IIndex index)
    {
        var columns = string.Join(", ", index.Properties.Select(p => $"'{p.Name}'"));
        return index.Properties.Count > 1
            ? $"Combination of [{columns}] is already being used"
            : $"{columns} is already being used";
    }

    public string? Resolve(UniqueConstraintException exception, DbContext dbContext)
    {
        var map = GetOrBuildUniqueConstraintMap(dbContext);
        if (map.TryGetValue(exception.ConstraintName, out var response))
            return response;
        logger.LogWarning("Unique constraint {UniqueConstraintName} not found in EF Core model.", exception.ConstraintName);
        return null;
    }
}