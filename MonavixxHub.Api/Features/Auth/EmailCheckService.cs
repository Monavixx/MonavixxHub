using System.Text.RegularExpressions;

namespace MonavixxHub.Api.Features.Auth;

public partial class EmailCheckService
{
    private static readonly Regex emailRegex = MyRegex();
    public bool Check(string email) => emailRegex.IsMatch(email);
    [GeneratedRegex(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")]
    private static partial Regex MyRegex();
}