using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MonavixxHub.Api.Features.Auth.Models.EntityTypeConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> u)
    {
        u.HasKey(c => c.Id);
        u.HasIndex(c => c.Email).IsUnique();
        u.HasIndex(c => c.Username).IsUnique();
        u.Property(c => c.Username).HasMaxLength(User.UsernameMaxLength);
        u.Property(c => c.Email).HasMaxLength(User.EmailMaxLength);
    }
}