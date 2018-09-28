using System.Threading.Tasks;

using WebApp.Dto.Common;
using WebApp.Dto.Movies;

namespace WebApp.Services.Movies
{
    public interface IMovieService
    {
        Task<Page<Movie>> GetMoviesAsync(MoviesQueryFilter queryFilter);

        Task<Movie> GetMovieAsync(int movieId);
    }
}
