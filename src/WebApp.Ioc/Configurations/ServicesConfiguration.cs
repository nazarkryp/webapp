using Microsoft.Extensions.DependencyInjection;

using WebApp.Services;
using WebApp.Services.Categories;
using WebApp.Services.Models;
using WebApp.Services.Movies;
using WebApp.Services.Studios;

namespace WebApp.Ioc.Configurations
{
    public static class ServicesConfiguration
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddTransient<IMediaService, MediaService>();
            services.AddTransient<IMovieService, MovieService>();
            services.AddTransient<IStudioService, StudioService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IModelService, ModelService>();
        }
    }
}
