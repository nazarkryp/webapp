using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;
using WebApp.Security.Configuration;

namespace WebApp.Security
{
    public class GoogleSecurityProvider : IGoogleSecurityProvider
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;

        public GoogleSecurityProvider(IGoogleConfiguration configuration)
        {
            _clientId = configuration.ClientId;
            _clientSecret = configuration.ClientSecret;
            _redirectUri = configuration.RedirectUri;
        }

        public string RedirectUri => $"https://accounts.google.com/o/oauth2/v2/auth?client_id={this._clientId}&scope=openid email profile&response_type=code&redirect_uri={_redirectUri}";

        public async Task<AccessToken> GetAccessToken(string code)
        {
            var requestUri = $"https://www.googleapis.com/oauth2/v4/token?code={code}&client_id={_clientId}&client_secret={_clientSecret}&redirect_uri={_redirectUri}&grant_type=authorization_code";

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(requestUri, null);
                var content = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<AccessToken>(content);
            }
        }
    }
}
