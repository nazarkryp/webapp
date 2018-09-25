using System.Collections.Generic;
using System.Threading.Tasks;

using WebApp.Dto.Studios;
using WebApp.Mapping;
using WebApp.Repositories.Repositories;
using WebApp.Services.Exceptions;

namespace WebApp.Services.Studios
{
    public class StudioService : IStudioService
    {
        private readonly IStudioRepository _studios;
        private readonly IMapper _mapper;

        public StudioService(IStudioRepository studios, IMapper mapper)
        {
            _studios = studios;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Studio>> GetStudiosAsync()
        {
            var studios = await _studios.FindStudiosAsync();

            return _mapper.Map<IEnumerable<Studio>>(studios);
        }

        public async Task<Studio> GetStudioByIdAsync(int studioId)
        {
            var studio = await _studios.FindAsync(studioId);

            if (studio == null)
            {
                throw new ResourceNotFoundException($"Studio {studioId} not found");
            }

            return _mapper.Map<Studio>(studio);
        }
    }
}
