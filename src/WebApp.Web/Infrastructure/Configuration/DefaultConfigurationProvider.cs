using Microsoft.Extensions.Configuration;

namespace WebApp.Web.Infrastructure.Configuration
{
    public class DefaultConfigurationProvider : WebApp.Infrastructure.Configuration.IConfigurationProvider
    {
        private readonly IConfiguration _configuration;

        public DefaultConfigurationProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Get(string key)
        {
            return _configuration[key];
        }
    }
}
