using System.Threading.Tasks;

using WebApp.Studios;

namespace WebApp.Jobs.Sync.Scrappers
{
    public interface IScrapper
    {
        Task ScrapMoviesAsync(params IStudioClient[] studioClients);
    }
}
