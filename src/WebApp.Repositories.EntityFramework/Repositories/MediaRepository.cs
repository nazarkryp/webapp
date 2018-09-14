using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using WebApp.Domain.Entities;
using WebApp.Mapping;
using WebApp.Repositories.EntityFramework.Context;
using WebApp.Repositories.Repositories;

namespace WebApp.Repositories.EntityFramework.Repositories
{
    public class MediaRepository : GenericRepository<Binding.Models.Media>, IMediaRepository
    {
        private readonly IMapper _mapper;

        public MediaRepository(IDbContext context, IMapper mapper)
            : base(context)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<Media>> FindAllMediaAsync()
        {
            var items = await FindAll().ToListAsync();

            return _mapper.Map<IEnumerable<Media>>(items);
        }

        public async Task<Media> AddMediaAsync(Media media)
        {
            var entity = _mapper.Map<Binding.Models.Media>(media);

            Add(entity);

            await SaveChangesAsync();

            return _mapper.Map<Media>(entity);
        }

        public async Task<Media> FindByIdAsync(int mediaId)
        {
            var entity = await FindAsync(e => e.MediaId == mediaId);

            return _mapper.Map<Media>(entity);
        }

        public async Task<IEnumerable<Media>> FindByIdsAsync(params int[] mediaIds)
        {
            var media = await FindAll(e => mediaIds.Any(id => id == e.MediaId)).ToListAsync();

            return _mapper.Map<IEnumerable<Media>>(media);
        }

        public Task RemoveMediaAsync(params int[] mediaIds)
        {
            var media = FindAll(e => mediaIds.Any(id => id == e.MediaId));

            RemoveRange(media);

            return SaveChangesAsync();
        }
    }
}
