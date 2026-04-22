using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MonavixxHub.Api.Features.Auth.Extensions;
using MonavixxHub.Api.Features.Flashcards.Models;
using MonavixxHub.Api.Features.Flashcards.Services;
using MonavixxHub.Api.Features.FlashcardStudy.DTOs;
using MonavixxHub.Api.Features.FlashcardStudy.Models;
using MonavixxHub.Api.Infrastructure;

namespace MonavixxHub.Api.Features.FlashcardStudy.Services;

public class RandomFlashcardStudyAlgorithmService (
    AppDbContext dbContext,
    ILogger<RandomFlashcardStudyAlgorithmService> logger)
{
    public async Task<Flashcard?> NextAsync(ClaimsPrincipal user, FlashcardSet flashcardSet)
    {
        logger.LogInformation("Running Random Flashcard Study Algorithm");
        var userId = user.GetUserId();
        
        var session = await GetOrCreateSession(userId, flashcardSet);
        var nextFlashcard = dbContext.FlashcardSetEntries
            .Where(e => e.FlashcardSetId == flashcardSet.Id)
            .Select(e => e.Flashcard)
            .Where(f => !dbContext.RandomStudySessions
                .Where(s => s.UserId == userId && s.FlashcardSetId == flashcardSet.Id)
                .SelectMany(s => s.AnsweredFlashcards)
                .Any(s => s.Id == f.Id))
            .OrderBy(_ => EF.Functions.Random())
            .FirstOrDefault();

        if (nextFlashcard is not null)
        {
            logger.LogInformation("Chosen flashcard id: {flashcardId}, front: {front}", nextFlashcard.Id,
                nextFlashcard.Front);
            session.AnsweredFlashcards.Add(nextFlashcard);
        }
        else
        {
            logger.LogInformation("No flashcard found, session will be removed");
            dbContext.RandomStudySessions.Remove(session);
        }
        await dbContext.SaveChangesAsync();
        return nextFlashcard;
    }

    private RandomStudySession CreateSession(UserIdType userId, FlashcardSet flashcardSet)
    {
        logger.LogInformation("New session will be created");
        var session = new RandomStudySession()
        {
            UserId = userId,
            FlashcardSetId = flashcardSet.Id
        };
        dbContext.RandomStudySessions.Add(session);
        return session;
    }

    public async Task<RandomStudySession> GetOrCreateSession(UserIdType userId, FlashcardSet flashcardSet)
    {
        return await dbContext.RandomStudySessions
                   .FirstOrDefaultAsync(s => s.UserId == userId && s.FlashcardSetId == flashcardSet.Id)
               ?? CreateSession(userId, flashcardSet);
    }

    public async Task<GetRandomStudySessionDto?> GetSessionDto(UserIdType userId, FlashcardSet flashcardSet)
    {
        return await dbContext.RandomStudySessions
            .Where(s => s.UserId == userId && s.FlashcardSetId == flashcardSet.Id)
            .Select(s => new GetRandomStudySessionDto(AnsweredCorrectly: s.AnsweredFlashcards.Count,
                Total: dbContext.FlashcardSetEntries.Count(e => e.FlashcardSetId == flashcardSet.Id)))
            .FirstOrDefaultAsync();
    }
}