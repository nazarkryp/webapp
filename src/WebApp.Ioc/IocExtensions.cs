using Microsoft.Extensions.DependencyInjection;

using WebApp.Infrastructure.Cache;
using WebApp.Infrastructure.Handlers;
using WebApp.Infrastructure.Parsers;

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
    }
}
