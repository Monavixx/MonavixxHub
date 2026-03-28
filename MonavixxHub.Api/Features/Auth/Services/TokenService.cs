using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MonavixxHub.Api.Features.Auth.Models;

namespace MonavixxHub.Api.Features.Auth.Services;

/// <summary>
/// Provides functionality to generate authentication tokens for users.
/// </summary>
public class TokenService (IConfiguration config)
{
    /// <summary>
    /// Generates a JWT for the specified user.
    /// </summary>
    /// <param name="user">The user for whom the token will be generated.</param>
    /// <returns>A string containing the JWT that can be used for authentication.</returns>
    public string GenerateToken(User user)
    {
        Claim[] claims =
        [
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
        ];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(config.GetValue<int>("Jwt:ExpiryHours")),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}