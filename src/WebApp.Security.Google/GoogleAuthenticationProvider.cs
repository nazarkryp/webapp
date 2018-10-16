using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json;

using WebApp.Infrastructure.Configuration;
using WebApp.Security.Configuration;
using WebApp.Security.Google.Models;
using WebApp.Security.Models;

namespace WebApp.Security.Google
{
    public class GoogleAuthenticationProvider : IAuthenticationProvider
    {
        private const string BaseAddess = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string ResponseType = "code";

        private readonly string[] _scopes = { "openid", "email", "profile" };

        private readonly IConfigurationProvider _configurationProvider;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;

        public GoogleAuthenticationProvider(IOAuthConfiguration configuration, IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
            _clientId = configuration.ClientId;
            _clientSecret = configuration.ClientSecret;
            _redirectUri = configuration.RedirectUri;
        }

        public string RedirectUri => $"{BaseAddess}" +
                                     $"?client_id={_clientId}" +
                                     $"&scope={string.Join(" ", _scopes)}" +
                                     $"&response_type={ResponseType}" +
                                     $"&redirect_uri={_redirectUri}";

        public async Task<string> GetAccessToken(string code)
        {
            const string TokenBaseAddress = "https://www.googleapis.com/oauth2/v4/token";

            var requestUri = $"{TokenBaseAddress}" +
                             $"?code={code}" +
                             $"&client_id={_clientId}" +
                             $"&client_secret={_clientSecret}" +
                             $"&redirect_uri={_redirectUri}" +
                             $"&grant_type=authorization_code";

            AccessToken accessToken;

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(requestUri, null);
                var content = await response.Content.ReadAsStringAsync();

                accessToken = JsonConvert.DeserializeObject<AccessToken>(content);
            }

            var claims = GetClaimsIdentity(accessToken);

            return CreateJwt(claims.ToList());
        }

        private static IEnumerable<Claim> GetClaimsIdentity(IAccessToken accessToken)
        {
            var jwtSecurityToken = new JwtSecurityToken(accessToken.IdToken);

            var claimsDictionary = jwtSecurityToken.Payload.Claims.ToDictionary(e => e.Type, e => e.Value);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, claimsDictionary[JwtRegisteredClaimNames.Email]),
                new Claim(JwtRegisteredClaimNames.GivenName, claimsDictionary[JwtRegisteredClaimNames.GivenName]),
                new Claim(JwtRegisteredClaimNames.FamilyName, claimsDictionary[JwtRegisteredClaimNames.FamilyName]),
                new Claim(JwtRegisteredClaimNames.Email, claimsDictionary[JwtRegisteredClaimNames.Email]),
                new Claim(JwtRegisteredClaimNames.Sub, claimsDictionary[JwtRegisteredClaimNames.Sub]),
                new Claim(JwtRegisteredClaimNames.Exp, DateTime.UtcNow.AddDays(1).ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, claimsDictionary[JwtRegisteredClaimNames.Iat]),
                new Claim("picture", claimsDictionary["picture"])
            };

            return claims;
        }

        public string CreateJwt(IList<Claim> claims)
        {
            var securityKeyString = _configurationProvider.Get("SecurityKey");
            var securityKey = Encoding.UTF8.GetBytes(securityKeyString);
            var key = new SymmetricSecurityKey(securityKey);

            var expires = claims.FirstOrDefault(e => e.Type == JwtRegisteredClaimNames.Exp)?.Value;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Parse(expires),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }
    }
}
