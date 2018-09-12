using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WebApp.Domain.Entities;
using WebApp.Mapping;
using WebApp.Repositories.EntityFramework.Context;
using WebApp.Repositories.Repositories;

namespace WebApp.Repositories.EntityFramework.Repositories
{
    public class StudioRepository : GenericRepository<Binding.Models.Studio>, IStudioRepository
    {
        private readonly IMapper _mapper;

        public StudioRepository(IDbContext context, IMapper mapper)
            : base(context)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<Studio>> FindStudiosAsync()
        {
            var entities = await FindAll(null, e => e.SyncDetails).ToListAsync();

            return _mapper.Map<IEnumerable<Studio>>(entities);
        }

        public async Task<Studio> FindAsync(int studioId)
        {
            var entity = await FindAsync(e => e.StudioId == studioId, e => e.SyncDetails);

            return _mapper.Map<Studio>(entity);
        }

        public async Task<Studio> FindAsync(string name)
        {
            var entity = await FindAsync(e => string.Equals(e.Name, name, StringComparison.CurrentCultureIgnoreCase), e => e.SyncDetails);

            return _mapper.Map<Studio>(entity);
        }

        public async Task<Studio> AddAsync(Studio studio)
        {
            var entity = _mapper.Map<Binding.Models.Studio>(studio);

            entity = Add(entity);

            await SaveChangesAsync();

            return _mapper.Map<Studio>(entity);
        }

        public async Task<SyncDetails> UpdateAsync(SyncDetails syncDetails)
        {
            var entity = await Context.Set<Binding.Models.SyncDetails>().FirstOrDefaultAsync(e => e.StudioId == syncDetails.StudioId);
            EntityEntry result;

            if (entity == null)
            {
                entity = _mapper.Map<Binding.Models.SyncDetails>(syncDetails);
                result = Context.Set<Binding.Models.SyncDetails>().Add(entity);
            }
            else
            {
                entity = _mapper.Map(syncDetails, entity);
                result = Context.Set<Binding.Models.SyncDetails>().Update(entity);
            }

            await SaveChangesAsync();

            return _mapper.Map<SyncDetails>(result.Entity);
        }
    }
}
