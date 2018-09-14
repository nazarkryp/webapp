﻿using System;
using Microsoft.Extensions.DependencyInjection;

using WebApp.Ioc;
using WebApp.Jobs.Sync.Configuration;
using WebApp.Jobs.Sync.Jobs;

namespace WebApp.Jobs.Sync.Infrastructure
{
    internal class IocConfig
    {
        public static IServiceProvider ConfigureIoc()
        {
            var services = new ServiceCollection();

            services.ConfigureMapping();
            services.ConfigureRepositories();
            services.ConfigureInfrastructure();
            services.ConfigureStudioClients();

            services.AddSingleton<ISyncConfiguration, SyncConfiguration>();
            services.AddTransient<IJob, SyncMoviesDataJob>();

            return services.BuildServiceProvider();
        }
    }
}
