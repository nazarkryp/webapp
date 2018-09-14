using System.Threading.Tasks;

using WebApp.Studios;

namespace WebApp.Jobs.Sync.Jobs
{
    internal interface IJob
    {
        Task SyncAsync(IStudioClient studioClient);
    }
}
