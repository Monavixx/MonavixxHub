namespace MonavixxHub.Api.Features.Flashcards.Authorization;

/// <summary>
/// Specifies the type of access a user has to a specific flashcard.
/// </summary>
public enum FlashcardAccessType
{
    /// <summary>
    /// User has read-only access to all flashcard fields.
    /// </summary>
    Read,
    /// <summary>
    /// User can edit fields that are allowed to be modified.
    /// </summary>
    Edit
}