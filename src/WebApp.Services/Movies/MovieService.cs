using System.Collections.Generic;
using System.Threading.Tasks;

using WebApp.Dto.Movies;
using WebApp.Infrastructure.Parsers;
using WebApp.Mapping;
using WebApp.Repositories.Movies;
using WebApp.Repositories.Repositories;
using WebApp.Services.Exceptions;

namespace WebApp.Services.Movies
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movies;
        private readonly IMapper _mapper;
        private readonly IOrderByFilterParser _orderByFilterParser;

        public MovieService(IMovieRepository movies, IMapper mapper, IOrderByFilterParser orderByFilterParser)
        {
            _movies = movies;
            _mapper = mapper;
            _orderByFilterParser = orderByFilterParser;
        }

        public async Task<Dto.Common.Page<Movie>> GetMoviesAsync(MoviesQueryFilter queryFilter)
        {
            var orderByFilters = _orderByFilterParser.Parse<Movie>(queryFilter?.Orderby) ?? new[] { nameof(Movie.Date) };

            var pagingFilter = new MoviesPagingFilter
            {
                OrderBy = orderByFilters,
                Page = queryFilter?.Page,
                Size = queryFilter?.Size,
                SearchQuery = queryFilter?.Search,
                StudioIds = queryFilter?.StudioId
            };

            var page = await _movies.GetPageAsync(pagingFilter);

            var result = new Dto.Common.Page<Movie>
            {
                Data = _mapper.Map<IEnumerable<Movie>>(page.Data),
                Size = page.Size,
                Offset = page.Offset,
                Total = page.Total
            };

            return result;
        }

        public async Task<Movie> GetMovieAsync(int movieId)
        {
            var movie = await _movies.FindMovieAsync(movieId);

            if (movie == null)
            {
                throw new ResourceNotFoundException("Movie not found");
            }

            return _mapper.Map<Movie>(movie);
        }
    }
}
