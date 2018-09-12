using System.Net.Http;
using System.Threading.Tasks;

using WebApp.Security.Configuration;
using WebApp.Security.Google.Models;
using WebApp.Security.Models;

using Newtonsoft.Json;

namespace WebApp.Security.Google
{
    public class GoogleAuthenticationProvider : IAuthenticationProvider
    {
        private const string BaseAddess = "https://accounts.google.com/o/oauth2/v2/auth";

        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;

        public GoogleAuthenticationProvider(IOAuthConfiguration configuration)
        {
            _clientId = configuration.ClientId;
            _clientSecret = configuration.ClientSecret;
            _redirectUri = configuration.RedirectUri;
        }

        public string RedirectUri => $"{BaseAddess}" +
                                     $"?client_id={_clientId}" +
                                     $"&scope=openid email profile" +
                                     $"&response_type=code" +
                                     $"&redirect_uri={_redirectUri}";

        public async Task<IAccessToken> GetAccessToken(string code)
        {
            const string TokenBaseAddress = "https://www.googleapis.com/oauth2/v4/token";

            var requestUri = $"{TokenBaseAddress}" +
                             $"?code={code}" +
                             $"&client_id={_clientId}" +
                             $"&client_secret={_clientSecret}" +
                             $"&redirect_uri={_redirectUri}" +
                             $"&grant_type=authorization_code";

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(requestUri, null);
                var content = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<AccessToken>(content);
            }
        }
    }
}
