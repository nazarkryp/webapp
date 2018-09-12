using Newtonsoft.Json;

namespace WebApp.Security.Models
{
    public interface IAccessToken
    {
        string Token { get; set; }

        string IdToken { get; set; }

        string ExpiresIn { get; set; }

        string Scope { get; set; }

        string TokenType { get; set; }
    }
}
