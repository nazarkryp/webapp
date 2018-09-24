using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using WebApp.Security.Validators;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace WebApp.Security.Google.Validators
{
    public class GoogleTokenValidator : ITokenValidator
    {
        const int MaxJwtSegmentCount = 5;
        const int JwsSegmentCount = 3;
        const int JweSegmentCount = 5;

        //private readonly string _clientId;

        //public GoogleTokenValidator(string clientId)
        //{
        //    _clientId = clientId;
        //}

        public bool CanValidateToken => true;

        public int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

        public bool CanReadToken(string token)
        {
            //var handler = new JwtSecurityTokenHandler();

            //return handler.CanReadToken(securityToken);

            if (string.IsNullOrWhiteSpace(token))
                return false;

            if (token.Length > MaximumTokenSizeInBytes)
            {
                return false;
            }

            var tokenParts = token.Split(new[] { '.' }, MaxJwtSegmentCount + 1);

            switch (tokenParts.Length)
            {
                case JwsSegmentCount:
                    return JwtTokenUtilities.RegexJws.IsMatch(token);
                case JweSegmentCount:
                    return JwtTokenUtilities.RegexJwe.IsMatch(token);
                default:
                    return false;
            }
        }

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            validatedToken = null;

            //var validateTokenAddress = $"https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={securityToken}";
            var validateIdToken = $"https://www.googleapis.com/oauth2/v1/tokeninfo?id_token={securityToken}";

            using (var client = new HttpClient())
            {
                var response = client.GetAsync(validateIdToken).Result;

                if (!response.IsSuccessStatusCode)
                {

                }

                var responseContent = response.Content.ReadAsStringAsync().Result;
            }

            var token = new JwtSecurityToken(securityToken);

            var claimsDictionary = token.Payload.Claims.ToDictionary(e => e.Type, e => e.Value);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, claimsDictionary[JwtRegisteredClaimNames.Email]),
                new Claim(ClaimTypes.Name, claimsDictionary[JwtRegisteredClaimNames.Email]),
                new Claim(JwtRegisteredClaimNames.GivenName, claimsDictionary[JwtRegisteredClaimNames.GivenName]),
                new Claim(JwtRegisteredClaimNames.FamilyName, claimsDictionary[JwtRegisteredClaimNames.FamilyName]),
                new Claim(JwtRegisteredClaimNames.Email, claimsDictionary[JwtRegisteredClaimNames.Email]),
                new Claim(JwtRegisteredClaimNames.Sub, claimsDictionary[JwtRegisteredClaimNames.Sub]),
                new Claim(JwtRegisteredClaimNames.Iss, claimsDictionary[JwtRegisteredClaimNames.Iss]),
            };

            var principal = new ClaimsPrincipal();

            principal.AddIdentity(new ClaimsIdentity(claims, "Password"));

            return principal;
        }
    }
}
