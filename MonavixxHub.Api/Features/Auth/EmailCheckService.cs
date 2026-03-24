using System.Text.RegularExpressions;

namespace MonavixxHub.Api.Features.Auth;

public class EmailCheckService
{
    private static Regex emailRegex = new Regex(@"^[a-zA-Z0-9_\.%+-]+?@[a-zA-Z0-9\.-]+\.[a-zA-Z]{2,}$");
    public bool Check(string email) => emailRegex.IsMatch(email);
}