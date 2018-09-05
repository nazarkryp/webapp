using System.Collections.Generic;
using System.Threading.Tasks;

using WebApp.Domain.Entities;

namespace WebApp.Repositories.Repositories
{
    public interface IMediaRepository
    {
        Task<IEnumerable<Media>> FindAllMediaAsync();

        Task<Media> AddMediaAsync(Media media);

        Task<Media> FindByIdAsync(int mediaId);

        Task<IEnumerable<Media>> FindByIdsAsync(params int[] mediaIds);

        Task RemoveMediaAsync(params int[] mediaIds);
    }
}
