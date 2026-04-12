using MonavixxHub.Api.Features.Auth.Models;

namespace MonavixxHub.Api.Features.Auth.Services;

/// <summary>
/// Provides user management operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <returns>Queryable collection of all users.</returns>
    IQueryable<User> GetAllUsers();

    /// <summary>
    /// Gets a paginated set of users.
    /// </summary>
    /// <param name="page">Zero-based page number.</param>
    /// <param name="limit">Number of users per page.</param>
    /// <returns>Queryable collection of users for the specified page.</returns>
    IQueryable<User> GetUsers(int page, int limit);

    /// <summary>
    /// Gets a user by ID.
    /// </summary>
    /// <param name="userId">The user ID to retrieve.</param>
    /// <returns>The user entity.</returns>
    /// <exception cref="UserDoesNotExistException">Thrown if the user does not exist.</exception>
    Task<User> GetUserAsync(UserIdType userId);

    /// <summary>
    /// Deletes a user by ID.
    /// </summary>
    /// <param name="userId">The user ID to delete.</param>
    /// <exception cref="UserDoesNotExistException">Thrown if the user does not exist.</exception>
    Task DeleteUser(UserIdType userId);

    /// <summary>
    /// Deletes multiple users.
    /// </summary>
    /// <param name="userIds">The set of user IDs to delete.</param>
    /// <exception cref="UserDoesNotExistException">Thrown if not all specified users exist.</exception>
    Task DeleteUsers(ISet<UserIdType> userIds);

    /// <summary>
    /// Bans a user account.
    /// </summary>
    /// <param name="user">The user to ban.</param>
    Task BanAsync(User user);
}

