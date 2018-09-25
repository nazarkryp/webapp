using System;
using System.Collections.Generic;
using System.Linq;
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

                //var studioClients = serviceProvider.GetServices<IStudioClient>();

                //await Task.WhenAll(studioClients.Select(job.SyncAsync));

                var studioClients = serviceProvider.GetServices<IStudioClient>().Where(e => e.StudioName == "Naughty America");

                await Task.WhenAll(studioClients.Select(job.SyncAsync));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
        }
    }
}
