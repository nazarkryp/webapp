﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using Microsoft.IdentityModel.Tokens;

namespace WebApp.Security.Validators
{
    public class GoogleTokenValidator : ISecurityTokenValidator
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public GoogleTokenValidator()
        {
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        public bool CanValidateToken => true;

        public int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

        public bool CanReadToken(string securityToken)
        {
            return _tokenHandler.CanReadToken(securityToken);
        }

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            validatedToken = null;

            var payload = GoogleJsonWebSignature.ValidateAsync(securityToken, new GoogleJsonWebSignature.ValidationSettings()).Result;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, payload.Name),
                new Claim(ClaimTypes.Name, payload.Name),
                new Claim(JwtRegisteredClaimNames.FamilyName, payload.FamilyName),
                new Claim(JwtRegisteredClaimNames.GivenName, payload.GivenName),
                new Claim(JwtRegisteredClaimNames.Email, payload.Email),
                new Claim(JwtRegisteredClaimNames.Sub, payload.Subject),
                new Claim(JwtRegisteredClaimNames.Iss, payload.Issuer),
            };

            try
            {
                var principle = new ClaimsPrincipal();
                principle.AddIdentity(new ClaimsIdentity(claims, "Password"));
                return principle;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;

            }
        }
    }
}
