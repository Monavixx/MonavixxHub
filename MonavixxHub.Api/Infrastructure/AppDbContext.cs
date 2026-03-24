using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Common.Models;
using MonavixxHub.Api.Features.Auth.Models;
using MonavixxHub.Api.Features.Flashcards.Models;

namespace MonavixxHub.Api.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(u =>
        {
            u.HasKey(c => c.Id);
            u.HasIndex(c => c.Email).IsUnique();
            u.HasIndex(c => c.Username).IsUnique();
        });
        modelBuilder.Entity<Image>(i =>
        {
            i.HasKey(c => c.Id);
        });
        modelBuilder.Entity<Flashcard>(f =>
        {
            f.HasKey(g => g.Id);
            f.HasOne(g => g.Owner)
                .WithMany()
                .HasForeignKey(g => g.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
            f.HasOne(g => g.Image)
                .WithMany()
                .HasForeignKey(g => g.ImageId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Image> Images => Set<Image>();
    public DbSet<Flashcard> Flashcards => Set<Flashcard>();
}