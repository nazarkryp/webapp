using System.Threading.Tasks;

using WebApp.Studios;

namespace WebApp.Jobs.Sync.Jobs
{
    public interface IDetailsJob
    {
        Task SyncMovieDetailsAsync(IStudioClient studioClient);
    }
}
