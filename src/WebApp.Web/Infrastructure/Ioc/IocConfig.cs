using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using WebApp.Ioc;

// using WebApp.Web.Messaging;

namespace WebApp.Web.Infrastructure.Ioc
{
    public static class IocConfig
    {
        public static void ConfigureIoc(IConfiguration configuration, IServiceCollection services)
        {
            services.ConfigureMapping();
            services.ConfigureRepositories();
            services.ConfigureCloudinary();
            services.ConfigureServices();
            services.ConfigureSecurity();
            services.ConfigureInfrastructure();
            //var queueConnectionString = configuration.GetConnectionString("StorageConnectionString");
            //services.AddSingleton<IEventPublisher, EventPublisher>(_ => new EventPublisher(queueConnectionString));
        }
    }
}
