using System.Collections.Concurrent;
using System.Collections.Frozen;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;

namespace MonavixxHub.Api.Common.Exceptions.Resolvers;

/// <summary>
/// Resolves <see cref="DbUpdateException"/> caused by unique constraint errors into a user-friendly message
/// describing which database column(s) caused the violation.
/// Thread-safe.
/// Note: doesn't work with Sqlite as it doesn't populate ConstraintName.
/// </summary>
public class UniqueConstraintResolver (ILogger<UniqueConstraintResolver> logger)
: ConstraintResolverBase<UniqueConstraintResolver>(logger)
{
    protected override FrozenDictionary<string, string> BuildMap(DbContext dbContext)
    {
        var map = new Dictionary<string, string>();
    
        foreach (var entityType in dbContext.Model.GetEntityTypes())
        {
            var indexes = entityType.GetIndexes().Where(i => i.IsUnique);
            foreach (var index in indexes)
                map[index.GetDatabaseName()!] = GetResponseByIndex(index);
            
            var pk = entityType.FindPrimaryKey();
            if (pk != null) map[pk.GetName()!] = GetResponseByPk(pk);
        }
    
        return map.ToFrozenDictionary();
    }

    private string GetResponseByIndex(IIndex index)
    {
        var columns = string.Join(", ", index.Properties.Select(p => $"'{p.GetColumnName()}'"));
        return index.Properties.Count > 1
            ? $"Combination of [{columns}] is already being used"
            : $"{columns} is already being used";
    }
    private string GetResponseByPk(IKey key)
    {
        var columns = string.Join(", ", key.Properties.Select(p => $"'{p.GetColumnName()}'"));
        return key.Properties.Count > 1
            ? $"Combination of [{columns}] is already being used"
            : $"{columns} is already being used";
    }
}