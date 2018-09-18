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

        public async Task<IEnumerable<Movie>> LatestAsync(int studioId)
        {
            var max = await Context.Set<Binding.Models.Movie>().Where(e => e.StudioId == studioId).MaxAsync(e => e.Date);
            var movies = await Context.Set<Binding.Models.Movie>().Where(e => e.StudioId == studioId && e.Date == max).ToListAsync();

            return _mapper.Map<IEnumerable<Movie>>(movies);
        }

        public async Task<Page<Movie>> GetPageAsync(IPagingFilter pagingFilter)
        {
            var query = Context.Set<Binding.Models.Movie>()
                .Include(e => e.Studio)
                .Include(e => e.Attachments)
                .Include(e => e.MovieModels)
                .ThenInclude(e => e.Model);

            var page = await GetPageAsync(query, pagingFilter.OrderBy, pagingFilter.Page, pagingFilter.Size);

            return new Page<Movie>
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
            var modelsNames = movies.SelectMany(e => e.Models).Select(e => e.Name).Distinct();

            var existingModels = await Context.Set<Binding.Models.Model>().ToListAsync();
            var models = existingModels;

            // var modelsToSave = modelsNames.Where(e => models.All(em => !string.Equals(em.Name, e, StringComparison.CurrentCultureIgnoreCase))).Select(name => new Binding.Models.Model { Name = name }).ToList();

            var modelsToSave = modelsNames.Where(s => models.All(m => s.Split().All(e => m.Name.Split().Any(n => n == e)))).Select(name => new Binding.Models.Model { Name = name }).ToList();

            if (modelsToSave.Any())
            {
                await Context.Set<Binding.Models.Model>().AddRangeAsync(modelsToSave);
                await SaveChangesAsync();

                existingModels = await Context.Set<Binding.Models.Model>().ToListAsync();
            }

            var entities = _mapper.Map<IList<Binding.Models.Movie>>(movies);

            MapReferences(existingModels, entities);

            await Context.Set<Binding.Models.Movie>().AddRangeAsync(entities);

            await SaveChangesAsync();

            return _mapper.Map<IEnumerable<Movie>>(entities);
        }

        private static void MapReferences(IReadOnlyCollection<Binding.Models.Model> existingModels, IEnumerable<Binding.Models.Movie> entities)
        {
            var movieModels = entities.SelectMany(e => e.MovieModels);

            foreach (var entityMovieModel in movieModels)
            {
                var model = existingModels.FirstOrDefault(existing => existing.Name.Split().All(e => entityMovieModel.Model.Name.Split().Any(n => n == e))); ;

                if (model != null)
                {
                    entityMovieModel.ModelId = model.ModelId;
                    entityMovieModel.Model.Name = null;
                }
            }
        }
    }
}
