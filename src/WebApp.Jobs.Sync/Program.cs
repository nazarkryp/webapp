using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
                //using (var client = new HttpClient(httpClientHandler))
                //{
                //    var response = await client.GetAsync("https://api.ipify.org/?format=json");
                //    var content = await response.Content.ReadAsStringAsync();
                //    Console.WriteLine(content);
                //}

                //return;


                var serviceProvider = IocConfig.ConfigureIoc();

                var job = serviceProvider.GetService<IJob>();
                var detailsJob = serviceProvider.GetService<IDetailsJob>();

                var studioClients = serviceProvider.GetServices<IStudioClient>();

                await detailsJob.SyncMovieDetailsAsync(studioClients?.FirstOrDefault(e => e.StudioName == "Naughty America"));

                //await studioClients?.FirstOrDefault(e => e.StudioName == "Brazzers").GetMovieDetailsAsync(
                //    "https://tour.brazzersnetwork.com/scenes/view/id/2971106/head-to-toe/");

                //await detailsJob.SyncMovieDetailsAsync(studioClients?.FirstOrDefault(e => e.StudioName == "Naughty America"));
                //await studioClients?.FirstOrDefault(e => e.StudioName == "Naughty America").GetMovieDetailsAsync(
                //    "https://tour.naughtyamerica.com/scene/would-you-accept-hot-milf-eva-long-as-payment-for-car-work-24647");

                //foreach (var studioClient in studioClients)
                //{
                //    await job.SyncAsync(studioClient);
                //}

                //await Task.WhenAll(studioClients.Select(job.SyncAsync));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
        }
    }
}
