using Microsoft.Extensions.DependencyInjection;

using WebApp.Mapping.AutoMapper.Mappers;

namespace WebApp.Mapping.AutoMapper
{
    public static class MappingModule
    {
        public static void ConfigureMapping(this IServiceCollection services)
        {
            services.AddSingleton<IMapper, MappingService>();
        }
    }
}
