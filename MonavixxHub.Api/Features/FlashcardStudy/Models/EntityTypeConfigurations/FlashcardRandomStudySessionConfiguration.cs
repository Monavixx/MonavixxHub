using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MonavixxHub.Api.Features.FlashcardStudy.Models.EntityTypeConfigurations;

public class FlashcardRandomStudySessionConfiguration : IEntityTypeConfiguration<FlashcardRandomStudySession>
{
    public void Configure(EntityTypeBuilder<FlashcardRandomStudySession> builder)
    {
        builder.HasKey(x => new { x.FlashcardId, x.UserId, x.FlashcardSetId });
    }
}