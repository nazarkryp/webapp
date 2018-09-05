using System.Collections.Generic;
using System.Threading.Tasks;

using WebApp.Dto;

namespace WebApp.Services
{
    public interface IMediaService
    {
        Task<IEnumerable<Media>> GetMediaAsync();

        Task<IEnumerable<Media>> GetMediaByIdsAsync(IEnumerable<int> mediaIds);

        Task<Media> CreateMediaAsync(MediaCreateOptions options);

        Task RemoveMediaAsync(MediaDeleteOptions options);
    }
}
