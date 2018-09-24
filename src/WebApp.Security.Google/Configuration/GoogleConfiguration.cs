using WebApp.Infrastructure.Configuration;
using WebApp.Security.Configuration;

namespace WebApp.Security.Google.Configuration
{
    public class GoogleConfiguration : IOAuthConfiguration
    {
        private const string ClientIdKey = "clientId";
        private const string ClientSecretKey = "clientSecret";
        private const string RedirectUriKey = "redirectUri";

        private readonly IConfigurationProvider _configurationProvider;

        private static string _clientId;
        private static string _clientSecret;
        private static string _redirectUri;

        public GoogleConfiguration(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public string ClientId => _clientId ?? (_clientId = _configurationProvider.Get(ClientIdKey));

        public string ClientSecret => _clientSecret ?? (_clientSecret = _configurationProvider.Get(ClientSecretKey));

        public string RedirectUri => _redirectUri ?? (_redirectUri = _configurationProvider.Get(RedirectUriKey));
    }
}
