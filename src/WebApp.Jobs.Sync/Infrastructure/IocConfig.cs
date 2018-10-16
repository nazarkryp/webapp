using System;

using Microsoft.Extensions.DependencyInjection;

using WebApp.Ioc;
using WebApp.Jobs.Sync.Configuration;
using WebApp.Jobs.Sync.Infrastructure.Communication;
using WebApp.Jobs.Sync.Jobs;
using WebApp.Jobs.Sync.Scrappers;

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
            services.AddTransient<IScrapper, MovieScrapper>();
            services.AddTransient<IDetailsJob, SyncMovieDetailsDataJob>();

            services.AddTransient<IScrapperConfiguration, ScrapperConfiguration>();
            services.AddTransient<IScrapperClient, ScrapperClient>();

            return services.BuildServiceProvider();
        }
    }
}
