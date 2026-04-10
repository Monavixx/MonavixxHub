using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MonavixxHub.Api.Features.Flashcards.Models.EntityTypeConfigurations;

public class FlashcardSetUserConfiguration : IEntityTypeConfiguration<FlashcardSetUser>
{
    public void Configure(EntityTypeBuilder<FlashcardSetUser> e)
    {
        e.HasKey(fsu => new { fsu.UserId, fsu.FlashcardSetId });
        e.HasOne(fsu => fsu.User)
            .WithMany(user => user.AddedFlashcardSets)
            .HasForeignKey(fsu => fsu.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        e.HasOne(fsu => fsu.FlashcardSet)
            .WithMany(fs => fs.Learners)
            .HasForeignKey(fsu => fsu.FlashcardSetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}