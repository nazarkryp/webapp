﻿using System.Collections.Generic;
using System.Threading.Tasks;

using WebApp.Dto.Common;
using WebApp.Dto.Movies;
using WebApp.Infrastructure.Parsers;
using WebApp.Mapping;
using WebApp.Repositories.Common;
using WebApp.Repositories.Repositories;

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

        public async Task<Dto.Common.Page<Movie>> GetMoviesAsync(QueryFilter queryFilter)
        {
            var orderByFilters = _orderByFilterParser.Parse<Movie>(queryFilter?.Orderby) ?? new[] { nameof(Movie.MovieId) };

            var pagingFilter = new PagingFilter
            {
                OrderBy = orderByFilters,
                Page = queryFilter?.Page,
                Size = queryFilter?.Size
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
    }
}