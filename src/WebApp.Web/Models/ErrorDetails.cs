using System.Collections.Generic;

using Newtonsoft.Json;

namespace WebApp.Web.Models
{
    public class ErrorDetails
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty("modelState", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<string> ModelState { get; set; }
    }
}
