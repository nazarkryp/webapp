using System.Collections.Generic;
using System.Threading.Tasks;

using WebApp.Domain.Entities;

namespace WebApp.Repositories.Repositories
{
    public interface IStudioRepository
    {
        Task<IEnumerable<Studio>> FindStudiosAsync();

        Task<Studio> FindAsync(int studioId);

        Task<Studio> FindAsync(string name);

        Task<Studio> AddAsync(Studio studio);

        Task<SyncDetails> UpdateAsync(SyncDetails syncDetails);
    }
}
