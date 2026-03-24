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
            u.Property(c => c.Username).HasMaxLength(200);
            u.Property(c => c.Email).HasMaxLength(320);
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
            f.Property(x => x.Front).HasMaxLength(250);
            f.Property(x => x.Back).HasMaxLength(250);
            f.Property(x => x.Transcription).HasMaxLength(250);
        });
        modelBuilder.Entity<FlashcardSetEntry>(e =>
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
        });
        modelBuilder.Entity<FlashcardSet>(e =>
        {
            e.HasOne(fs => fs.ParentSet)
                .WithMany(fs => fs.Subsets)
                .HasForeignKey(fs => fs.ParentSetId)
                .OnDelete(DeleteBehavior.Restrict);
            e.Property(x => x.Name).HasMaxLength(150);
            e.Property(x => x.IsPublic).HasDefaultValue(false);
        });
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Image> Images => Set<Image>();
    public DbSet<Flashcard> Flashcards => Set<Flashcard>();
    public DbSet<FlashcardSetEntry> FlashcardSetEntries => Set<FlashcardSetEntry>();
    public DbSet<FlashcardSet> FlashcardSets => Set<FlashcardSet>();
}