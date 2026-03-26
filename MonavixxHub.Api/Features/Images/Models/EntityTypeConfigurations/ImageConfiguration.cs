using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MonavixxHub.Api.Features.Images.Models.EntityTypeConfigurations;

public class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.MimeType).HasMaxLength(Image.MimeTypeMaxLength);
    }
}