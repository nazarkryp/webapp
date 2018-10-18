using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using WebApp.Repositories.EntityFramework.Context;
using WebApp.Repositories.EntityFramework.Repositories;
using WebApp.Repositories.Repositories;

namespace WebApp.Ioc.Configurations
{
    public static class RepositoriesConfiguration
    {
        public static void ConfigureRepositories(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<WebAppDbContext>(options => options.UseSqlServer(connectionString));
            services.AddTransient<IMediaRepository, MediaRepository>();
            services.AddTransient<IModelRepository, ModelRepository>();
            services.AddTransient<IStudioRepository, StudioRepository>();
            services.AddTransient<IMovieRepository, MovieRepository>();
            services.AddTransient<ISyncDetailsRepository, SyncDetailsRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IDbContext, WebAppDbContext>();
        }
    }
}
