using MonavixxHub.Api.Features.FlashcardStudy.Models;

namespace MonavixxHub.Api.Features.FlashcardStudy.DTOs;

public record GetRandomStudySessionDto(
    int AnsweredCorrectly,
    int Total
);