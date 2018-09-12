using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp.Domain.Entities;

namespace WebApp.Repositories.Repositories
{
    public interface ISyncDetailsRepository
    {
        Task<SyncDetails> FindByStudioAsync(int studioId);

        Task<SyncDetails> AddAsync(SyncDetails syncDetails);

        Task<SyncDetails> UpdateAsync(SyncDetails syncDetails);
    }
}
