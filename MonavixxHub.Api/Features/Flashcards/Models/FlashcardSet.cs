using System.ComponentModel.DataAnnotations;
using MonavixxHub.Api.Features.Auth.Models;

namespace MonavixxHub.Api.Features.Flashcards.Models;

public class FlashcardSet
{
    public const int NameMaxLength = 150;
    [Required] public Guid Id { get; set; }
    [Required] public string Name { get; set; } = string.Empty;
    public List<FlashcardSetEntry> Entries { get; } = [];
    public FlashcardSet? ParentSet { get; set; } = null;
    public Guid? ParentSetId { get; set; } = null;
    [Required] public bool IsPublic  { get; set; } = false;
    [Required] public UserIdType OwnerId { get; set; }
    public User Owner { get; set; }
    public ICollection<FlashcardSet> Subsets { get; } = [];
}