using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp.Domain.Entities;
using WebApp.Dto.Movies;
using WebApp.Infrastructure.Parsers;
using WebApp.Mapping;
using WebApp.Repositories.Movies;
using WebApp.Repositories.Repositories;
using WebApp.Services.Exceptions;
using Movie = WebApp.Dto.Movies.Movie;

namespace WebApp.Services.Movies
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movies;
        private readonly ICategoryRepository _categories;
        private readonly IMapper _mapper;
        private readonly IOrderByFilterParser _orderByFilterParser;

        public MovieService(IMovieRepository movies, ICategoryRepository categories, IMapper mapper, IOrderByFilterParser orderByFilterParser)
        {
            _movies = movies;
            _mapper = mapper;
            _orderByFilterParser = orderByFilterParser;
            _categories = categories;
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
                Studios = queryFilter?.Studios,
                Categories = queryFilter?.Categories,
                Models = queryFilter?.Models,
                Date = queryFilter?.Date
            };

            var page = await _movies.FindMoviesAsync(pagingFilter);

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

        public async Task AssignMovieCategoryAsync(int movieId, int categoryId)
        {
            var movie = await _movies.FindMovieAsync(movieId);
            var category = await _categories.FindCategoryAsync(categoryId);

            movie.Categories.Add(category);

            var movies = new List<Domain.Entities.Movie>
            {
                movie
            };

            await _movies.UpdateAsync(movies);
        }
    }
}
