using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Entities;
using WebApp.Mapping;
using WebApp.Repositories.EntityFramework.Context;
using WebApp.Repositories.Repositories;

namespace WebApp.Repositories.EntityFramework.Repositories
{
    public class MovieRepository : GenericRepository<Binding.Models.Movie>, IMovieRepository
    {
        private readonly IMapper _mapper;

        public MovieRepository(IDbContext context, IMapper mapper)
            : base(context)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<Movie>> FindMoviesAsync()
        {
            var entities = await FindAll().ToListAsync();

            return _mapper.Map<IEnumerable<Movie>>(entities);
        }

        public async Task<Movie> AddAsync(Movie movie)
        {
            var entity = _mapper.Map<Binding.Models.Movie>(movie);

            entity = Add(entity);

            await SaveChangesAsync();

            return _mapper.Map<Movie>(entity);
        }

        public async Task<IEnumerable<Movie>> AddRangeAsync(IEnumerable<Movie> movies)
        {
            var entities = _mapper.Map<IEnumerable<Binding.Models.Movie>>(movies);

            // Context.Set<Binding.Models.Model>().Where(e => e.Name.ToLower() == entities.All(m => m.mo))
            var models = entities.SelectMany(e => e.MovieModels).Select(e => e.Model);
            var existingModels = await Context.Set<Binding.Models.Model>().Where(e => models.Any(m => string.Equals(m.Name, e.Name, StringComparison.CurrentCultureIgnoreCase))).ToListAsync();

            foreach (var existingModel in existingModels)
            {
                var model = models.FirstOrDefault(e => e.Name.ToLower() == existingModel.Name.ToLower());
                model.ModelId = existingModel.ModelId;
            }

            foreach (var entity in entities)
            {
                Add(entity);
            }

            await SaveChangesAsync();

            return _mapper.Map<IEnumerable<Movie>>(entities);
        }
    }
}
