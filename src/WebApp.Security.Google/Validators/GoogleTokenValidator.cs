using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using Google.Apis.Util;
using Microsoft.IdentityModel.Tokens;
using WebApp.Security.Validators;

namespace WebApp.Security.Google.Validators
{
    public class GoogleTokenValidator : ITokenValidator
    {
        private readonly string _clientId;

        public GoogleTokenValidator(string clientId)
        {
            _clientId = clientId;
        }

        public bool CanValidateToken => true;

        public int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

        public bool CanReadToken(string securityToken)
        {
            try
            {
                var token = new JwtSecurityToken(securityToken);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            validatedToken = null;

            //var validateTokenAddress = $"https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={securityToken}";
            //var validateIdToken = $"https://www.googleapis.com/oauth2/v1/tokeninfo?id_token={securityToken}";

            //using (var client = new HttpClient())
            //{
            //    var response = client.GetAsync(validateIdToken).Result;

            //    if (!response.IsSuccessStatusCode)
            //    {

            //    }
            //}

            var token = new JwtSecurityToken(securityToken);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "lanafeshchuk@gmail.com"),
                new Claim(ClaimTypes.Name, "lanafeshchuk"),
                new Claim(JwtRegisteredClaimNames.FamilyName, "Feshchuk"),
                new Claim(JwtRegisteredClaimNames.GivenName, "Ruslana"),
                new Claim(JwtRegisteredClaimNames.Email, "lanafeshchuk@gmail.com"),
                new Claim(JwtRegisteredClaimNames.Sub, token.Subject),
                new Claim(JwtRegisteredClaimNames.Iss, token.Issuer),
            };

            var principal = new ClaimsPrincipal();

            principal.AddIdentity(new ClaimsIdentity(claims, "Password"));

            return principal;
        }
    }
}
