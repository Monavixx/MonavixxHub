using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Features.FlashcardStudy.Models.EntityTypeConfigurations;

public class RandomStudySessionConfiguration : IEntityTypeConfiguration<RandomStudySession>
{
    public void Configure(EntityTypeBuilder<RandomStudySession> builder)
    {
        builder.HasKey(r => new { r.UserId, r.FlashcardSetId });
        builder.HasOne(r => r.FlashcardSet)
            .WithMany()
            .HasForeignKey(r => r.FlashcardSetId);
        builder.HasOne(r => r.User)
            .WithOne()
            .HasForeignKey<RandomStudySession>(r => r.UserId);
        builder.HasMany(r => r.AnsweredFlashcards)
            .WithMany()
            .UsingEntity<FlashcardRandomStudySession>(
                r => r.HasOne<Flashcard>()
                    .WithMany()
                    .HasForeignKey(f => f.FlashcardId)
                    .OnDelete(DeleteBehavior.Cascade),
                l => l.HasOne<RandomStudySession>()
                    .WithMany()
                    .HasForeignKey(f => new { f.UserId, f.FlashcardSetId })
                    .OnDelete(DeleteBehavior.Cascade)
            );
    }
}