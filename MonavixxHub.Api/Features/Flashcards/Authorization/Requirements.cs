namespace MonavixxHub.Api.Features.Flashcards.Authorization;

public static class Requirements
{
    public static class FlashcardSet
    {
        public static readonly FlashcardSetAccessRequirement ReadAccess =
            new(FlashcardSetAccessType.Read);

        public static readonly FlashcardSetAccessRequirement EditAccess =
            new(FlashcardSetAccessType.Edit);
    }

    public static class Flashcard
    {
        public static readonly FlashcardAccessRequirement ReadAccess =
            new(FlashcardAccessType.Read);

        public static readonly FlashcardAccessRequirement EditAccess =
            new(FlashcardAccessType.Edit);
    }
}