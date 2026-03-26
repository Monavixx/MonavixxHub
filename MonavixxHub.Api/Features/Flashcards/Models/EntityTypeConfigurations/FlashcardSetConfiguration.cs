using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MonavixxHub.Api.Features.Flashcards.Models.EntityTypeConfigurations;

public class FlashcardSetConfiguration : IEntityTypeConfiguration<FlashcardSet>
{
    public void Configure(EntityTypeBuilder<FlashcardSet> e)
    {
        e.HasOne(fs => fs.ParentSet)
            .WithMany(fs => fs.Subsets)
            .HasForeignKey(fs => fs.ParentSetId)
            .OnDelete(DeleteBehavior.Restrict);
        e.Property(x => x.Name).HasMaxLength(FlashcardSet.NameMaxLength);
        e.Property(x => x.IsPublic).HasDefaultValue(false);
    }
}