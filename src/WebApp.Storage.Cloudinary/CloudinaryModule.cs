using Microsoft.Extensions.DependencyInjection;

using WebApp.Storage.Cloudinary.Configuration;

namespace WebApp.Storage.Cloudinary
{
    public static class CloudinaryModule
    {
        public static void ConfigureCloudinary(this IServiceCollection services)
        {
            services.AddSingleton<ICloudinaryConfiguration, CloudinaryConfiguration>();
            services.AddScoped<IStorage<string>, CloudinaryStorage>();
        }
    }
}
