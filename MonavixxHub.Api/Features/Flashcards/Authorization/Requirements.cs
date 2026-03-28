using Microsoft.AspNetCore.Authorization;

namespace MonavixxHub.Api.Features.Flashcards.Authorization;

/// <summary>
/// Provides predefined instances of <see cref="FlashcardAccessRequirement"/> and 
/// <see cref="FlashcardSetAccessRequirement"/> for use in authorization policies.
/// </summary>
/// <remarks>
/// This class centralizes commonly used access requirements for flashcards and flashcard sets.
/// Use these instances in ASP.NET Core authorization policies or when invoking
/// <see cref="AuthorizationHandler{TRequirement, TResource}"/> handlers.
/// </remarks>
public static class Requirements
{
    /// <summary>
    /// Predefined access requirements for <see cref="FlashcardSet"/> resources.
    /// </summary>
    public static class FlashcardSet
    {
        /// <summary>
        /// Read-only access requirement for a flashcard set.
        /// Corresponds to <see cref="FlashcardSetAccessType.Read"/>.
        /// </summary>
        public static readonly FlashcardSetAccessRequirement ReadAccess =
            new(FlashcardSetAccessType.Read);

        /// <summary>
        /// Edit access requirement for a flashcard set.
        /// Corresponds to <see cref="FlashcardSetAccessType.Edit"/>.
        /// </summary>
        public static readonly FlashcardSetAccessRequirement EditAccess =
            new(FlashcardSetAccessType.Edit);
    }

    /// <summary>
    /// Predefined access requirements for <see cref="Flashcard"/> resources.
    /// </summary>
    public static class Flashcard
    {
        /// <summary>
        /// Read-only access requirement for a flashcard.
        /// Corresponds to <see cref="FlashcardAccessType.Read"/>.
        /// </summary>
        public static readonly FlashcardAccessRequirement ReadAccess =
            new(FlashcardAccessType.Read);

        /// <summary>
        /// Edit access requirement for a flashcard.
        /// Corresponds to <see cref="FlashcardAccessType.Edit"/>.
        /// </summary>
        public static readonly FlashcardAccessRequirement EditAccess =
            new(FlashcardAccessType.Edit);
    }
}