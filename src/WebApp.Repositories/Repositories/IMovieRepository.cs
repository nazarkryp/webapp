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

        Task<Movie> FindMovieAsync(int movieId);

        Task<Movie> AddAsync(Movie movie);

        Task<Movie> UpdateAsync(Movie movie);

        Task UpdateAsync(IEnumerable<Movie> movies);

        Task<IEnumerable<Movie>> AddRangeAsync(IEnumerable<Movie> movie);

        Task<IEnumerable<Movie>> FindAllStudioMoviesAsync(int studioId);
    }
}
