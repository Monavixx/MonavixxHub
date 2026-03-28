using System.Collections.Frozen;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MonavixxHub.Api.Common.Exceptions.Resolvers;

public class ForeignKeyConstraintResolver (ILogger<ForeignKeyConstraintResolver> logger)
: ConstraintResolverBase<ForeignKeyConstraintResolver>(logger)
{
    protected override FrozenDictionary<string, string> BuildMap(DbContext dbContext)
        => dbContext.Model
            .GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys())
            .Where(f => f.GetConstraintName() is not null)
            .ToFrozenDictionary(f => f.GetConstraintName()!, GetResponseByFk);
    private string GetResponseByFk(IForeignKey key)
    {
        var columns = string.Join(", ", key.Properties.Select(p => $"'{p.GetColumnName()}'"));
        return key.Properties.Count > 1
            ? $"Combination of [{columns}] refers to a non-existent entity"
            : $"{columns} refers to a non-existent entity";
    }
}