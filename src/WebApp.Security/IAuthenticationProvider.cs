using System.Threading.Tasks;

using WebApp.Security.Models;

namespace WebApp.Security
{
    public interface IAuthenticationProvider
    {
        string RedirectUri { get; }

        Task<IAccessToken> GetAccessToken(string code);
    }
}
