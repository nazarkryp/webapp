using System.Threading.Tasks;

using WebApp.Domain.Entities;
using WebApp.Mapping;
using WebApp.Repositories.EntityFramework.Context;
using WebApp.Repositories.Repositories;

namespace WebApp.Repositories.EntityFramework.Repositories
{
    public class SyncDetailsRepository : GenericRepository<Binding.Models.SyncDetails>, ISyncDetailsRepository
    {
        private readonly IMapper _mapper;

        public SyncDetailsRepository(IDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<SyncDetails> FindByStudioAsync(int studioId)
        {
            var entity = await FindAsync(e => e.StudioId == studioId);

            return _mapper.Map<SyncDetails>(entity);
        }

        public async Task<SyncDetails> AddAsync(SyncDetails syncDetails)
        {
            var entity = _mapper.Map<Binding.Models.SyncDetails>(syncDetails);

            Add(entity);

            await SaveChangesAsync();

            return _mapper.Map<SyncDetails>(entity);
        }

        public async Task<SyncDetails> UpdateAsync(SyncDetails syncDetails)
        {
            var entity = await FindAsync(e => e.SyncDetailsId == syncDetails.SyncDetailsId);

            entity = _mapper.Map(syncDetails, entity);

            var result = Context.Set<Binding.Models.SyncDetails>().Update(entity);

            await SaveChangesAsync();

            return _mapper.Map<SyncDetails>(result.Entity);
        }
    }
}
