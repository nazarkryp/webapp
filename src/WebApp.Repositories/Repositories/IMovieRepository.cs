using System.Collections.Generic;
using System.Threading.Tasks;

using WebApp.Domain.Entities;

namespace WebApp.Repositories.Repositories
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>> FindMoviesAsync();

        Task<Movie> AddAsync(Movie movie);

        Task<IEnumerable<Movie>> AddRangeAsync(IEnumerable<Movie> movie);
    }
}
