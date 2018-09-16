using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using WebApp.Domain.Entities;
using WebApp.Mapping;
using WebApp.Repositories.Common;
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

        public async Task<Movie> LastAsync()
        {
            var movie = await Context.Set<Binding.Models.Movie>().LastOrDefaultAsync();

            return _mapper.Map<Movie>(movie);
        }

        public async Task<Page<Movie>> GetPageAsync(IPagingFilter pagingFilter)
        {
            var query = Context.Set<Binding.Models.Movie>()
                .Include(e => e.Studio)
                .Include(e => e.Attachments)
                .Include(e => e.MovieModels)
                .ThenInclude(e => e.Model);

            var page = await GetPageAsync(query, pagingFilter.OrderBy, pagingFilter.Page, pagingFilter.Size);

            return new Page<Movie>()
            {
                Size = page.Size,
                Data = _mapper.Map<IEnumerable<Movie>>(page.Data),
                Offset = page.Offset,
                Total = page.Total
            };
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
            var names = entities.SelectMany(e => e.MovieModels).Select(e => e.Model.Name).Distinct();

            var existingModels = await Context.Set<Binding.Models.Model>().ToListAsync();
            var modelsToSave = names.Where(e => existingModels.All(em => !string.Equals(em.Name, e, StringComparison.CurrentCultureIgnoreCase))).Select(name => new Binding.Models.Model { Name = name });

            if (modelsToSave.Any())
            {
                await Context.Set<Binding.Models.Model>().AddRangeAsync(modelsToSave);
                await SaveChangesAsync();
            }

            existingModels = await Context.Set<Binding.Models.Model>().ToListAsync();

            foreach (var entity in entities)
            {
                foreach (var entityMovieModel in entity.MovieModels)
                {
                    entityMovieModel.ModelId = existingModels.First(e => e.Name == entityMovieModel.Model.Name).ModelId;
                    entityMovieModel.Model.Name = null;
                }

                Add(entity);
            }

            await SaveChangesAsync();

            return _mapper.Map<IEnumerable<Movie>>(entities);
        }
    }
}
