using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MonavixxHub.Api.Features.FlashcardsStudy.Models.EntityTypeConfigurations;

public class RandomStudySessionConfiguration : IEntityTypeConfiguration<RandomStudySession>
{
    public void Configure(EntityTypeBuilder<RandomStudySession> builder)
    {
        builder.HasKey(r => new { r.UserId, r.FlashcardSetId });
        builder.HasOne(r => r.FlashcardSet)
            .WithMany()
            .HasForeignKey(r => r.FlashcardSetId);
        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId);
        builder.HasOne(r => r.LastFlashcard)
            .WithMany()
            .HasForeignKey(r => r.LastFlashcardId);
    }
}