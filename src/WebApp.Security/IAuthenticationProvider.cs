using System.Threading.Tasks;

namespace WebApp.Security
{
    public interface IAuthenticationProvider
    {
        string RedirectUri { get; }

        Task<string> GetAccessToken(string code);
    }
}
