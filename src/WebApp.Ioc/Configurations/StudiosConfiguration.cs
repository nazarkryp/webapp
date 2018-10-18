using Microsoft.Extensions.DependencyInjection;

using WebApp.Studios;
using WebApp.Studios.Studio1;
using WebApp.Studios.Studio2;
using WebApp.Studios.Studio3;

namespace WebApp.Ioc.Configurations
{
    public static class StudiosConfiguration
    {
        public static void ConfigureStudioClients(this IServiceCollection services)
        {
            services.AddTransient<IStudioClient, Studio1Client>();
            services.AddTransient<IStudioClient, Studio2Client>();
            services.AddTransient<IStudioClient, Studio3Client>();
        }
    }
}
