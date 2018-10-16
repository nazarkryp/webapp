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

namespace WebApp.Jobs.Sync
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var serviceProvider = IocConfig.ConfigureIoc();
                var studioClients = serviceProvider.GetServices<IStudioClient>().Where(e => !string.IsNullOrEmpty(e.StudioName));
                //var job = serviceProvider.GetService<IJob>();

                var detailsJob = serviceProvider.GetService<IDetailsJob>();
                var scrapper = serviceProvider.GetService<IScrapper>();
                await scrapper.ScrapMoviesAsync(studioClients.ToArray());

                //foreach (var studioClient in studioClients)
                //{
                //    Console.WriteLine($"Scrapping: {studioClient.StudioName}");
                //    await job.SyncAsync(studioClient);
                //}
                //await Task.WhenAll(studioClients.Select(job.SyncAsync));

                var getDetailsTasks = new List<Task>
                {
                    /*detailsJob.SyncMovieDetailsAsync(studioClients?.FirstOrDefault(e => e.StudioName == Studio1ClientConstants.StudioName)),*/
                    detailsJob.SyncMovieDetailsAsync(studioClients?.FirstOrDefault(e => e.StudioName == Studio2ClientConstants.StudioName))
                };
                await Task.WhenAll(getDetailsTasks);

                getDetailsTasks = new List<Task>
                {
                    detailsJob.SyncMovieDetailsAsync(studioClients?.FirstOrDefault(e => e.StudioName == Studio1ClientConstants.StudioName))
                };

                await Task.WhenAll(getDetailsTasks);

                //await Task.WhenAll(studioClients.Select(detailsJob.SyncMovieDetailsAsync));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
        }
    }
}
