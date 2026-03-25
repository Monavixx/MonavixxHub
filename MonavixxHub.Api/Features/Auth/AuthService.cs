using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Features.Auth.DTOs;
using MonavixxHub.Api.Features.Auth.Exceptions;
using MonavixxHub.Api.Features.Auth.Models;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.Auth;

public class AuthService(
    TokenService tokenService,
    AppDbContext dbContext,
    PasswordHashService passwordHashService,
    EmailCheckService emailCheckService)
{
    public async Task<AuthResponseDto> LoginAsync(string usernameOrEmail, string password)
    {
        User user;
        if (emailCheckService.Check(usernameOrEmail))
        {
            user = (await dbContext.Users.SingleOrDefaultAsync(u => u.Email == usernameOrEmail))!;
        }
        else
        {
            user = (await dbContext.Users.SingleOrDefaultAsync(u => u.Username == usernameOrEmail))!;
        }
        if (user is null)
            throw new UserDoesNotExistException();
        if (passwordHashService.Verify(password, user.PasswordHash))
            return new AuthResponseDto(tokenService.GenerateToken(user), user.Username);
        throw new WrongUsernameOrPasswordException();
    }

    public async Task<AuthResponseDto> RegisterAsync(string username, string password, string email)
    {
        if (await dbContext.Users.AnyAsync(u => u.Email == email))
            throw new UserWithSuchEmailAlreadyExistsException();
        if (await dbContext.Users.AnyAsync(u => u.Username == username))
            throw new UserWithSuchUsernameAlreadyExistsException();
        User user = new User()
        {
            Email = email,
            Username = username,
            PasswordHash = passwordHashService.Hash(password),
            CreatedAt = DateTimeOffset.UtcNow
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        return new AuthResponseDto(tokenService.GenerateToken(user), user.Username);
    }
}