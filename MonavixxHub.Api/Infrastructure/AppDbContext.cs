using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Features.Auth.Models;
using MonavixxHub.Api.Features.Flashcards.Models;
using MonavixxHub.Api.Features.FlashcardStudy.Models;
using MonavixxHub.Api.Features.Images.Models;

namespace MonavixxHub.Api.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Image> Images => Set<Image>();
    public DbSet<Flashcard> Flashcards => Set<Flashcard>();
    public DbSet<FlashcardSetEntry> FlashcardSetEntries => Set<FlashcardSetEntry>();
    public DbSet<FlashcardSet> FlashcardSets => Set<FlashcardSet>();
    public DbSet<FlashcardSetUser> FlashcardSetUsers => Set<FlashcardSetUser>();
    public DbSet<RandomStudySession> RandomStudySessions => Set<RandomStudySession>();
    public DbSet<FlashcardRandomStudySession> FlashcardRandomStudySessions => Set<FlashcardRandomStudySession>();
    
}