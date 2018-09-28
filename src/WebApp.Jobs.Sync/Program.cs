using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using WebApp.Jobs.Sync.Infrastructure;
using WebApp.Jobs.Sync.Jobs;
using WebApp.Studios;

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

                await detailsJob.SyncMovieDetailsAsync(studioClients?.FirstOrDefault(e => e.StudioName == "Brazzers"));

                //var movie = await studioClients?.FirstOrDefault(e => e.StudioName == "Brazzers")?.GetMovieDetailsAsync("https://tour.brazzersnetwork.com/scenes/view/id/2892769/angel-tits/");

                //foreach (var studioClient in studioClients)
                //{
                //    await job.SyncAsync(studioClient);
                //}

                // await Task.WhenAll(studioClients.Select(job.SyncAsync));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
        }
    }
}
