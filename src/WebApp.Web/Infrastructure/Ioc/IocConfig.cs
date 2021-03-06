﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using WebApp.Ioc;
using WebApp.Ioc.Configurations;
using WebApp.Web.Infrastructure.Configuration;

namespace WebApp.Web.Infrastructure.Ioc
{
    public static class IocConfig
    {
        public static void ConfigureIoc(IConfiguration configuration, IServiceCollection services)
        {
            var connectionString = configuration["connectionString"];

            services.ConfigureMapping();
            services.ConfigureRepositories(connectionString);
            services.ConfigureCloudinary();
            services.ConfigureServices();
            services.ConfigureSecurity();
            services.ConfigureInfrastructure();

            services.AddSingleton<WebApp.Infrastructure.Configuration.IConfigurationProvider, DefaultConfigurationProvider>();
            //var queueConnectionString = configuration.GetConnectionString("StorageConnectionString");
            //services.AddSingleton<IEventPublisher, EventPublisher>(_ => new EventPublisher(queueConnectionString));
        }
    }
}
