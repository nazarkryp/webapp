using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApp.Studios
{
    public interface IPage
    {
        int PageIndex { get; set; }

        Task<IEnumerable<IMovie>> MoviesTask { get; set; }
    }
}
