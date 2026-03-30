using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Features.Auth.Exceptions;
using MonavixxHub.Api.Features.Auth.Models;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Auth.Services;

public class UserService (AppDbContext dbContext)
{
    public IQueryable<User> GetAllUsers()
    {
        return dbContext.Users;
    }

    public IQueryable<User> GetUsers(int page, int limit)
    {
        return GetAllUsers().Skip(page * limit).Take(limit);
    }

    public async Task DeleteUser(UserIdType userId)
    {
        // dbContext.Flashcards.Where(f => f.OwnerId == userId)
        if(await dbContext.Users.Where(u => u.Id == userId).ExecuteDeleteAsync() is 0)
            throw new UserDoesNotExistException();
    }

    public async Task DeleteUsers(ISet<UserIdType> userIds)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        var deleted = await dbContext.Users.Where(u => userIds.Contains(u.Id)).ExecuteDeleteAsync();
        if(deleted == userIds.Count)
            await transaction.CommitAsync();
        else
        {
            await transaction.RollbackAsync();
            throw new UserDoesNotExistException();
        }
    }
}