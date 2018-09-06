﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using WebApp.Mapping.AutoMapper;
using WebApp.Repositories.EntityFramework;
using WebApp.Security;
using WebApp.Security.Configuration;
using WebApp.Services;
using WebApp.Storage.Cloudinary;
using WebApp.Web.Infrastructure.Security;

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

            services.AddSingleton<IGoogleConfiguration, GoogleConfiguration>();
            services.AddTransient<IGoogleSecurityProvider, GoogleSecurityProvider>();

            //var queueConnectionString = configuration.GetConnectionString("StorageConnectionString");
            //services.AddSingleton<IEventPublisher, EventPublisher>(_ => new EventPublisher(queueConnectionString));
        }
    }
}
