using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MonavixxHub.Api.Common.Exceptions;
using MonavixxHub.Api.Common.Options;
using MonavixxHub.Api.Features.Auth;
using MonavixxHub.Api.Features.Auth.Middlewares;
using MonavixxHub.Api.Features.Flashcards.Authorization;
using MonavixxHub.Api.Features.Flashcards.Controllers;
using MonavixxHub.Api.Features.Flashcards.DTOs;
using MonavixxHub.Api.Features.Flashcards.Services;
using MonavixxHub.Api.Features.Images.Authorization;
using MonavixxHub.Api.Features.Images.Services;
using MonavixxHub.Api.Infrastructure;
using Scalar.AspNetCore;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<EmailCheckService>();
builder.Services.AddSingleton<PasswordHashService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddOpenApi();
builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("Storage"));
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<FlashcardService>();
builder.Services.AddScoped<FlashcardSetService>();
builder.Services.AddScoped<FlashcardAccessService>();
builder.Services.AddScoped<ImageAccessService>();
builder.Services.AddSingleton<IAuthorizationHandler, FlashcardSetAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, FlashcardAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ImageAuthorizationHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();


builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(optionsBuilder =>
{
    optionsBuilder.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseMiddleware<ValidateUserMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();