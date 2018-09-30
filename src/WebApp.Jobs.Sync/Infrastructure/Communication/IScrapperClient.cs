using System.Threading.Tasks;

namespace WebApp.Jobs.Sync.Infrastructure.Communication
{
    public interface IScrapperClient
    {
        Task<string> GetAsync(string uri);
    }
}
