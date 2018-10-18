using Microsoft.Extensions.DependencyInjection;

using WebApp.Security;
using WebApp.Security.Configuration;
using WebApp.Security.Google;
using WebApp.Security.Google.Configuration;

namespace WebApp.Ioc.Configurations
{
    public static class SecurityConfiguration
    {
        public static void ConfigureSecurity(this IServiceCollection services)
        {
            services.AddSingleton<IOAuthConfiguration, GoogleConfiguration>();
            services.AddTransient<IAuthenticationProvider, GoogleAuthenticationProvider>();
        }
    }
}
