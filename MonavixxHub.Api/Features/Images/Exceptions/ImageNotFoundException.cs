using System.Net;
using MonavixxHub.Api.Common.Exceptions;

namespace MonavixxHub.Api.Features.Images.Exceptions;

public class ImageNotFoundException() : AppBaseException("Image not found", HttpStatusCode.NotFound);