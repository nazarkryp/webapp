using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebApp.Dto;
using WebApp.Mapping;
using WebApp.Repositories.Repositories;

namespace WebApp.Services
{
    internal class MediaService : IMediaService
    {
        private readonly IMediaRepository _media;
        private readonly IMapper _mapper;

        public MediaService(IMediaRepository media, IMapper mapper)
        {
            _media = media;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Media>> GetMediaAsync()
        {
            var media = await _media.FindAllMediaAsync();

            return _mapper.Map<IEnumerable<Media>>(media);
        }

        public async Task<IEnumerable<Media>> GetMediaByIdsAsync(IEnumerable<int> mediaIds)
        {
            var media = await _media.FindByIdsAsync(mediaIds.ToArray());

            return _mapper.Map<IEnumerable<Media>>(media);
        }

        public async Task<Media> CreateMediaAsync(MediaCreateOptions options)
        {
            var media = _mapper.Map<Domain.Entities.Media>(options);
            media = await _media.AddMediaAsync(media);

            return _mapper.Map<Media>(media);
        }

        public async Task RemoveMediaAsync(MediaDeleteOptions options)
        {
            await _media.RemoveMediaAsync(options.MediaIds.ToArray());
        }
    }
}
