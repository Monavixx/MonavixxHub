global using UserIdType = int;

using System.ComponentModel.DataAnnotations;
using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.Auth.Models;

/// <summary>
/// Represents an application user.
/// </summary>
public class User
{
    /// <summary>
    /// Maximum length of email address.
    /// </summary>
    public const int EmailMaxLength = 254;

    /// <summary>
    /// Maximum length of username.
    /// </summary>
    public const int UsernameMaxLength = 150;

    /// <summary>
    /// Minimum length of username.
    /// </summary>
    public const int UsernameMinLength = 3;

    /// <summary>
    /// Minimum length of password.
    /// </summary>
    public const int PasswordMinLength = 8;

    /// <summary>
    /// Maximum length between username and email.
    /// </summary>
    public const int UsernameOrEmailMaxLength = EmailMaxLength > UsernameMaxLength ? EmailMaxLength : UsernameMaxLength;

    /// <summary>
    /// Unique identifier of the user.
    /// </summary>
    [Required] 
    public UserIdType Id { get; set; }

    /// <summary>
    /// Unique username of the user.
    /// </summary>
    [Required] 
    public string Username { get; set; }

    /// <summary>
    /// Hashed password of the user.
    /// </summary>
    [Required] 
    public byte[] PasswordHash { get; set; }

    /// <summary>
    /// Unique email of the user.
    /// </summary>
    [Required] 
    public string Email { get; set; }

    /// <summary>
    /// Timestamp when the user was created.
    /// </summary>
    [Required] 
    public DateTimeOffset CreatedAt { get; set; }
    
    [Required] public UserRole Role { get; set; } = UserRole.User;
    [Required] public bool IsBanned { get; set; } = false;
    
    [Required] public bool IsEmailConfirmed { get; set; } = false;
    public byte[]? EmailConfirmationToken { get; set; }
    public DateTimeOffset? EmailConfirmationTokenExpiresAt { get; set; }
    
    public ICollection<Session> Sessions { get; } = [];
    public ICollection<FlashcardSetUser> AddedFlashcardSets { get; } = [];
}