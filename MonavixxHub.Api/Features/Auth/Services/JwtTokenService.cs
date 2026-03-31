using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MonavixxHub.Api.Features.Auth.Models;

namespace MonavixxHub.Api.Features.Auth.Services;

/// <summary>
/// Provides functionality to generate authentication tokens for users.
/// </summary>
public class JwtTokenService
{
    /// <summary>
    /// Generates a JWT for the specified user.
    /// </summary>
    /// <param name="user">The user for whom the token will be generated.</param>
    /// <returns>A string containing the JWT that can be used for authentication.</returns>
    public (string, DateTimeOffset) GenerateToken(User user)
    {
        Claim[] claims =
        [
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
        ];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var expires = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("Jwt:ExpiryMinutes"));
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature));
        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
    private readonly TokenValidationParameters _validationParameters;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly IConfiguration _config;

    /// <summary>
    /// Provides functionality to generate authentication tokens for users.
    /// </summary>
    public JwtTokenService(IConfiguration config)
    {
        _config = config;
        _tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]!);
        _validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    }

    public ClaimsPrincipal? Validate(string token)
    {
        try
        {
            return _tokenHandler.ValidateToken(token, _validationParameters, out _);
        }
        catch
        {
            return null;
        }
    }
}