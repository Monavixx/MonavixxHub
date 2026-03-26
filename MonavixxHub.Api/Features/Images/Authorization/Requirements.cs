namespace MonavixxHub.Api.Features.Images.Authorization;

public static class Requirements
{
    public static ImageReadAccessRequirement ReadAccess { get; } = new ();
}