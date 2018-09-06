using System.Threading.Tasks;

namespace WebApp.Security
{
    public interface IGoogleSecurityProvider
    {
        string RedirectUri { get; }

        Task<AccessToken> GetAccessToken(string code);
    }
}
