using Microsoft.Extensions.DependencyInjection;

using WebApp.Mapping;
using WebApp.Mapping.AutoMapper.Mappers;

namespace WebApp.Ioc.Configurations
{
    public static class MappingConfiguration
    {
        public static void ConfigureMapping(this IServiceCollection services)
        {
            services.AddSingleton<IMapper, MappingService>();
        }
    }
}
