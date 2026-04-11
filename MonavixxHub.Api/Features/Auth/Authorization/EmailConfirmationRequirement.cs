using Microsoft.AspNetCore.Authorization;
using MonavixxHub.Api.Common.Authorization;

namespace MonavixxHub.Api.Features.Auth.Authorization;

public class EmailConfirmationRequirement : IAuthorizationRequirement
{
    public static EmailConfirmationRequirement Default = new(); // todo: сделать так же с другими requirements
}