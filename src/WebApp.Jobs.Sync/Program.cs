using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using WebApp.Jobs.Sync.Infrastructure;
using WebApp.Jobs.Sync.Jobs;
using WebApp.Jobs.Sync.Scrappers;
using WebApp.Studios;
using WebApp.Studios.Studio1;
using WebApp.Studios.Studio2;
using WebApp.Studios.Studio3;

namespace WebApp.Jobs.Sync
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var serviceProvider = IocConfig.ConfigureIoc("Data Source=.;Initial Catalog=WebApp;Integrated Security=True;Connect Timeout=60;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
                var studioClients = serviceProvider.GetServices<IStudioClient>().Where(e => !string.IsNullOrEmpty(e.StudioName)).ToList();
                var detailsJob = serviceProvider.GetService<IDetailsJob>();
                var scrapper = serviceProvider.GetService<IScrapper>();
                await scrapper.ScrapMoviesAsync(studioClients.ToArray());

                var getDetailsTasks = new List<Task>
                {
                    detailsJob.SyncMovieDetailsAsync(studioClients?.FirstOrDefault(e => e.StudioName == Studio2ClientConstants.StudioName))
                };
                await Task.WhenAll(getDetailsTasks);

                getDetailsTasks = new List<Task>
                {
                    detailsJob.SyncMovieDetailsAsync(studioClients?.FirstOrDefault(e => e.StudioName == Studio1ClientConstants.StudioName))
                };

                await Task.WhenAll(getDetailsTasks);

                getDetailsTasks = new List<Task>
                {
                    detailsJob.SyncMovieDetailsAsync(studioClients?.FirstOrDefault(e => e.StudioName == Studio3ClientConstants.StudioName))
                };

                await Task.WhenAll(getDetailsTasks);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
        }
    }
}
