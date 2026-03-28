namespace MonavixxHub.Api.Features.Flashcards.Authorization;

/// <summary>
/// Specifies the type of access a user has to a flashcard set.
/// </summary>
/// <remarks>
/// Used in conjunction with <see cref="FlashcardSetAccessRequirement"/>
/// to enforce authorization policies for flashcard sets.
/// </remarks>
public enum FlashcardSetAccessType
{
    /// <summary>
    /// User has read-only access to the flashcard set.
    /// </summary>
    Read,
    
    /// <summary>
    /// User can edit the flashcard set, including adding or modifying entries.
    /// </summary>
    Edit
}