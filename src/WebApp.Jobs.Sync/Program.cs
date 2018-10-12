using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using WebApp.Jobs.Sync.Infrastructure;
using WebApp.Jobs.Sync.Jobs;
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

                var job = serviceProvider.GetService<IJob>();
                var detailsJob = serviceProvider.GetService<IDetailsJob>();
                var studioClients = serviceProvider.GetServices<IStudioClient>();

                foreach (var studioClient in studioClients)
                {
                    Console.WriteLine($"Scrapping: {studioClient.StudioName}");
                    await job.SyncAsync(studioClient);
                }

                //await Task.WhenAll(studioClients.Select(job.SyncAsync));

                //await studioClients?.FirstOrDefault(e => e.StudioName == Studio1ClientConstants.StudioName).GetMovieDetailsAsync("https://tour.brazzersnetwork.com/scenes/view/id/2871343/slow-and-sexy/");

                //var getDetailsTasks = new List<Task>
                //{
                //    // detailsJob.SyncMovieDetailsAsync(studioClients?.FirstOrDefault(e => e.StudioName == Studio1ClientConstants.StudioName)),
                //    detailsJob.SyncMovieDetailsAsync(studioClients?.FirstOrDefault(e => e.StudioName == Studio2ClientConstants.StudioName))
                //};

                //await Task.WhenAll(getDetailsTasks);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
        }
    }
}
