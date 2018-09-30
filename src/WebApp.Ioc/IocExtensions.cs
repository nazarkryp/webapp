using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using WebApp.Infrastructure.Cache;
using WebApp.Infrastructure.Handlers;
using WebApp.Infrastructure.Parsers;
using WebApp.Mapping;
using WebApp.Mapping.AutoMapper.Mappers;
using WebApp.Repositories.EntityFramework.Context;
using WebApp.Repositories.EntityFramework.Repositories;
using WebApp.Repositories.Repositories;
using WebApp.Security;
using WebApp.Security.Configuration;
using WebApp.Security.Google;
using WebApp.Security.Google.Configuration;
using WebApp.Services;
using WebApp.Services.Categories;
using WebApp.Services.Movies;
using WebApp.Services.Studios;
using WebApp.Storage;
using WebApp.Storage.Cloudinary;
using WebApp.Storage.Cloudinary.Configuration;
using WebApp.Studios;
using WebApp.Studios.Studio1;
using WebApp.Studios.Studio2;

namespace WebApp.Ioc
{
    public static class IocExtensions
    {
        public static void ConfigureInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IConcurrentActionHandler, ConcurrentActionHandler>();
            services.AddTransient<ICacheStore, MemoryCacheStore>();
            services.AddTransient<IOrderByFilterParser, OrderByFilterParser>();
        }

        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddDbContext<WebAppDbContext>(options => options.UseSqlServer("Data Source=.;Initial Catalog=WebApp;Integrated Security=True;Connect Timeout=60;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"));
            services.AddTransient<IMediaRepository, MediaRepository>();
            services.AddTransient<IStudioRepository, StudioRepository>();
            services.AddTransient<IMovieRepository, MovieRepository>();
            services.AddTransient<ISyncDetailsRepository, SyncDetailsRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IDbContext, WebAppDbContext>();
        }

        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddTransient<IMediaService, MediaService>();
            services.AddTransient<IMovieService, MovieService>();
            services.AddTransient<IStudioService, StudioService>();
            services.AddTransient<ICategoryService, CategoryService>();
        }

        public static void ConfigureCloudinary(this IServiceCollection services)
        {
            services.AddSingleton<ICloudinaryConfiguration, CloudinaryConfiguration>();
            services.AddScoped<IStorage<string>, CloudinaryStorage>();
        }

        public static void ConfigureMapping(this IServiceCollection services)
        {
            services.AddSingleton<IMapper, MappingService>();
        }

        public static void ConfigureSecurity(this IServiceCollection services)
        {
            services.AddSingleton<IOAuthConfiguration, GoogleConfiguration>();
            services.AddTransient<IAuthenticationProvider, GoogleAuthenticationProvider>();
        }

        public static void ConfigureStudioClients(this IServiceCollection services)
        {
            services.AddTransient<IStudioClient, Studio1Client>();
            services.AddTransient<IStudioClient, Studio2Client>();
        }
    }
}
