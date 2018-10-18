using Microsoft.Extensions.DependencyInjection;

using WebApp.Storage;
using WebApp.Storage.Cloudinary;
using WebApp.Storage.Cloudinary.Configuration;

namespace WebApp.Ioc.Configurations
{
    public static class CloudinaryConfiguration
    {
        public static void ConfigureCloudinary(this IServiceCollection services)
        {
            services.AddSingleton<ICloudinaryConfiguration, Storage.Cloudinary.Configuration.CloudinaryConfiguration>();
            services.AddScoped<IStorage<string>, CloudinaryStorage>();
        }
    }
}
