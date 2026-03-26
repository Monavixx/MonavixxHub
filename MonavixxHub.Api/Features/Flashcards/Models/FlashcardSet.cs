using MonavixxHub.Api.Features.Auth.Models;

namespace MonavixxHub.Api.Features.Flashcards.Models;

public class FlashcardSet
{
    public const int NameMaxLength = 150;
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public IList<FlashcardSetEntry> Entries { get; } = [];
    public FlashcardSet? ParentSet { get; set; } = null;
    public Guid? ParentSetId { get; set; } = null;
    public bool IsPublic  { get; set; } = false;
    public int OwnerId { get; set; }
    public User Owner { get; set; }
    public ICollection<FlashcardSet> Subsets { get; } = [];
}