using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApp.Studios.Brazzers.Models
{
    public class BrazzersPage : IPage
    {
        public int PageIndex { get; set; }

        public Task<IEnumerable<IMovie>> MoviesTask { get; set; }
    }
}
