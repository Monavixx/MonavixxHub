using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MonavixxHub.Api.Features.Flashcards.Models.EntityTypeConfigurations;

public class FlashcardConfiguration : IEntityTypeConfiguration<Flashcard>
{
    public void Configure(EntityTypeBuilder<Flashcard> f)
    {
        f.HasKey(g => g.Id);
        f.HasOne(g => g.Owner)
            .WithMany()
            .HasForeignKey(g => g.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);
        f.HasOne(g => g.Image)
            .WithMany(i=>i.Flashcards)
            .HasForeignKey(g => g.ImageId)
            .OnDelete(DeleteBehavior.SetNull);
        f.Property(x => x.Front).HasMaxLength(Flashcard.FrontMaxLength);
        f.Property(x => x.Back).HasMaxLength(Flashcard.BackMaxLength);
        f.Property(x => x.Transcription).HasMaxLength(Flashcard.TranslationMaxLength);
    }
}