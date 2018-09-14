using System;
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
                var studioClients = serviceProvider.GetServices<IStudioClient>();

                foreach (var studioClient in studioClients)
                {
                    await job.SyncAsync(studioClient);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
