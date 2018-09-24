using System.Collections.Generic;
using System.Threading.Tasks;

using WebApp.Dto.Studios;

namespace WebApp.Services.Studios
{
    public interface IStudioService
    {
        Task<IEnumerable<Studio>> GetStudiosAsync();

        Task<Studio> GetStudioByIdAsync(int studioId);
    }
}
