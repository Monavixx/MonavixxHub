using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Features.Auth.Models;
using MonavixxHub.Api.Features.Flashcards.Models;
using MonavixxHub.Api.Features.FlashcardsStudy.Models;
using MonavixxHub.Api.Features.Images.Models;

namespace MonavixxHub.Api.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Image> Images => Set<Image>();
    public DbSet<Flashcard> Flashcards => Set<Flashcard>();
    public DbSet<FlashcardSetEntry> FlashcardSetEntries => Set<FlashcardSetEntry>();
    public DbSet<FlashcardSet> FlashcardSets => Set<FlashcardSet>();
    public DbSet<RandomStudySession> RandomStudySessions => Set<RandomStudySession>();
}