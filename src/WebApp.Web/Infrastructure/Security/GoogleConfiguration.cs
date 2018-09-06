using WebApp.Security.Configuration;

namespace WebApp.Web.Infrastructure.Security
{
    public class GoogleConfiguration : IGoogleConfiguration
    {
        public string ClientId => "918518562893-19gsgkiuolsfuhmephemj5pt7co42sv0.apps.googleusercontent.com";

        public string ClientSecret => "aecweSQ3qBXvnkIuQbp3x9Y0";

        public string RedirectUri => "https://localhost:44397/v1/account/callback";
    }
}
