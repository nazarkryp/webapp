using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using WebApp.Repositories.EntityFramework.Context;
using WebApp.Repositories.EntityFramework.Repositories;
using WebApp.Repositories.Repositories;

namespace WebApp.Repositories.EntityFramework
{
    public static class RepositoriesModule
    {
        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddDbContext<WebAppDbContext>(options => options.UseSqlServer("Data Source=.;Initial Catalog=WebApp;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"));
            services.AddTransient<IMediaRepository, MediaRepository>();
            services.AddTransient<IDbContext, WebAppDbContext>();
        }
    }
}
