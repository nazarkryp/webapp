using System.Collections.Generic;
using System.Threading.Tasks;

using WebApp.Domain.Entities;
using WebApp.Repositories.Common;
using WebApp.Repositories.Movies;

namespace WebApp.Repositories.Repositories
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>> LatestAsync(int studioId);

        Task<Page<Movie>> GetPageAsync(MoviesPagingFilter pagingFilter);

        Task<Movie> AddAsync(Movie movie);

        Task<IEnumerable<Movie>> AddRangeAsync(IEnumerable<Movie> movie);
    }
}
