﻿using WebApp.Security.Models;

using Newtonsoft.Json;

namespace WebApp.Security.Google.Models
{
    public class AccessToken : IAccessToken
    {
        [JsonProperty("access_token")]
        public string Token { get; set; }

        [JsonProperty("id_token")]
        public string IdToken { get; set; }

        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }
}
