using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MonavixxHub.Api.Features.Flashcards.Models.EntityTypeConfigurations;

public class FlashcardSetEntryConfiguration : IEntityTypeConfiguration<FlashcardSetEntry>
{
    public void Configure(EntityTypeBuilder<FlashcardSetEntry> e)
    {
        e.HasKey(fse => new { fse.FlashcardId, fse.FlashcardSetId });
        e.HasOne(fse => fse.Flashcard)
            .WithMany(fs => fs.Entries)
            .HasForeignKey(fse => fse.FlashcardId)
            .OnDelete(DeleteBehavior.Cascade);
        e.HasOne(fse => fse.FlashcardSet)
            .WithMany(fs => fs.Entries)
            .HasForeignKey(fse => fse.FlashcardSetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}