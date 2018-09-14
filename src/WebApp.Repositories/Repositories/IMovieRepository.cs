using System.Collections.Generic;
using System.Threading.Tasks;

using WebApp.Domain.Entities;
using WebApp.Repositories.Common;

namespace WebApp.Repositories.Repositories
{
    public interface IMovieRepository
    {
        Task<Page<Movie>> GetPageAsync(IPagingFilter pagingFilter);

        Task<Movie> AddAsync(Movie movie);

        Task<IEnumerable<Movie>> AddRangeAsync(IEnumerable<Movie> movie);
    }
}
