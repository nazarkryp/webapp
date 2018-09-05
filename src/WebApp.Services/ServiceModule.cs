using Microsoft.Extensions.DependencyInjection;

namespace WebApp.Services
{
    public static class ServiceModule
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddTransient<IMediaService, MediaService>();
        }
    }
}
