using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Channels;
using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Features.Auth.Exceptions;

namespace MonavixxHub.Api.Common.Exceptions;

public static class UniqueConstraintExceptionExtensions
{
    private static readonly ConcurrentDictionary<Type, Dictionary<string, Func<AppBaseException>>>
        CacheUniqueConstraints = new();

    private static Dictionary<string, Func<AppBaseException>> GetOrBuildUniqueConstraintMap(DbContext dbContext)
    {
        var dbType = dbContext.GetType();
        if (CacheUniqueConstraints.TryGetValue(dbType, out var map))
            return map;
        return CacheUniqueConstraints[dbType] = BuildUniqueConstraintMap(dbContext);
    }

    private static Dictionary<string, Func<AppBaseException>> BuildUniqueConstraintMap(DbContext dbContext)
    {
        var map = new Dictionary<string, Func<AppBaseException>>();
        foreach (var entityType in dbContext.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.PropertyInfo?.GetCustomAttribute<UniqueDomainExceptionAttribute>(false) is { } attribute)
                {
                    var index = entityType.GetIndexes()
                        .FirstOrDefault(i => i.Properties.Contains(property) && i.IsUnique);
                    if (index != null)
                    {
                        map[index.GetDatabaseName()!] = () => attribute.GetException();
                    }
                }
            }
        }
        return map;
    }
    public static AppBaseException? MapToDomain(this UniqueConstraintException exception, DbContext dbContext)
    {
        var map = GetOrBuildUniqueConstraintMap(dbContext);
        var message = exception.ConstraintName;
        foreach (var (key, func) in map)
        {
            if (message == key)
            {
                return func();
            }
        }
        return null;
    }
}