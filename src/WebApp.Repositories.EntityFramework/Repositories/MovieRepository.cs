using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using WebApp.Domain.Entities;
using WebApp.Mapping;
using WebApp.Repositories.Common;
using WebApp.Repositories.EntityFramework.Context;
using WebApp.Repositories.Movies;
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

        public async Task<IEnumerable<Movie>> FindAllStudioMoviesAsync(int studioId)
        {
            var movies = await Context.Set<Binding.Models.Movie>()
                .Include(e => e.MovieModels)
                .Include(e => e.MovieCategories)
                //.Include(e => e.Attachments)
                .Where(e => e.StudioId == studioId && !e.MovieCategories.Any()).OrderByDescending(e => e.MovieId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<Movie>>(movies);
        }

        public async Task<Page<Movie>> GetPageAsync(MoviesPagingFilter pagingFilter)
        {
            IQueryable<Binding.Models.Movie> query = Context.Set<Binding.Models.Movie>();

            if (!string.IsNullOrEmpty(pagingFilter?.SearchQuery))
            {
                query = query.Where(e => e.Title.ToLower().Contains(pagingFilter.SearchQuery) || e.Description.ToLower().Contains(pagingFilter.SearchQuery));
            }

            if (pagingFilter?.Studios != null && pagingFilter.Studios.Length > 0)
            {
                query = query.Where(e => pagingFilter.Studios.Distinct().Contains(e.StudioId));
            }

            query = query
                .Include(e => e.Studio)
                .Include(e => e.Attachments);

            var page = await GetPageAsync(query, pagingFilter?.OrderBy, pagingFilter?.Page, pagingFilter?.Size);

            return new Page<Movie>
            {
                Size = page.Size,
                Data = _mapper.Map<IEnumerable<Movie>>(page.Data),
                Offset = page.Offset,
                Total = page.Total
            };
        }

        public async Task<Page<Movie>> GetCategoriesMoviesAsync(MoviesPagingFilter pagingFilter)
        {
            if (pagingFilter?.Categories == null || pagingFilter.Categories.Length < 1)
            {
                return new Page<Movie>();
            }

            var categoriesNames = pagingFilter.Categories.Select(e => e.ToLower()).ToArray();

            var categories = await Context.Set<Binding.Models.Category>().Where(e => categoriesNames.Contains(e.Name.ToLower())).ToListAsync();

            if (categories.Count < 1)
            {
                return new Page<Movie>();
            }

            var categoriesIds = categories.Select(e => e.CategoryId).ToArray();

            //var query = Context.Set<Binding.Models.MovieCategory>()
            //    .Join(Context.Set<Binding.Models.Movie>(), mc => mc.MovieId, m => m.MovieId, (category, movie) => new { category, movie })
            //    .GroupBy(cmm => cmm.movie)
            //    .Where(grouped => grouped.Count(x => categoriesIds.Contains(x.category.CategoryId)) == categoriesIds.Count())
            //    .Select(cm => cm.Key);

            int[] idsQuery;

            if (pagingFilter?.Studios != null && pagingFilter.Studios.Length > 0)
            {
                idsQuery = await Context.Set<Binding.Models.MovieCategory>()
                    .Where(e => pagingFilter.Studios.Distinct().Contains(e.Movie.StudioId))
                    .GroupBy(cmm => cmm.MovieId)
                    .Where(grouped => grouped.Count(x => categoriesIds.Contains(x.CategoryId)) == categoriesIds.Count())
                    .Select(cm => cm.Key).ToArrayAsync();
            }
            else
            {
                idsQuery = await Context.Set<Binding.Models.MovieCategory>()
                    .GroupBy(cmm => cmm.MovieId)
                    .Where(grouped => grouped.Count(x => categoriesIds.Contains(x.CategoryId)) == categoriesIds.Count())
                    .Select(cm => cm.Key).ToArrayAsync();
            }


            var query = Context.Set<Binding.Models.Movie>()
                .Include(e => e.Attachments)
                .Include(e => e.Studio)
                .Where(e => idsQuery.Contains(e.MovieId));

            var page = await GetPageAsync(query, pagingFilter.OrderBy, pagingFilter.Page, pagingFilter.Size);

            return new Page<Movie>
            {
                Size = page.Size,
                Data = _mapper.Map<IEnumerable<Movie>>(page.Data),
                Offset = page.Offset,
                Total = page.Total
            };
        }

        public async Task<Page<Movie>> FindMoviesAsync(MoviesPagingFilter pagingFilter)
        {
            IQueryable<int> moviesIds;

            if (pagingFilter.Studios?.Length >= 1)
            {
                moviesIds = Context.Set<Binding.Models.Movie>()
                    .Where(e => pagingFilter.Studios.Contains(e.StudioId))
                    .Select(e => e.MovieId);
            }
            else
            {
                moviesIds = Context.Set<Binding.Models.Movie>().Select(e => e.MovieId);
            }

            if (pagingFilter.Models?.Length >= 1)
            {
                moviesIds = Context.Set<Binding.Models.MovieModel>()
                    .Join(moviesIds, e => e.MovieId, e => e, (movie, i) => new { movie, i })
                    .GroupBy(e => e.i)
                    .Where(grouped => grouped.Count(x => pagingFilter.Models.Contains(x.movie.ModelId)) == pagingFilter.Models.Count())
                    .Select(e => e.Key);
            }

            if (pagingFilter.Categories?.Length >= 1)
            {
                var categoriesNames = pagingFilter.Categories.Select(e => e.ToLower()).ToArray();
                var categories = await Context.Set<Binding.Models.Category>().Where(e => categoriesNames.Contains(e.Name.ToLower())).ToListAsync();
                var categoriesIds = categories.Select(e => e.CategoryId).ToArray();

                moviesIds = Context.Set<Binding.Models.MovieCategory>()
                    .Join(moviesIds, e => e.MovieId, e => e, (movie, i) => new { movie, i })
                    .GroupBy(cmm => cmm.i)
                    .Where(grouped => grouped.Count(x => categoriesIds.Contains(x.movie.CategoryId)) == categoriesIds.Count())
                    .Select(cm => cm.Key);
            }

            var query = Context.Set<Binding.Models.Movie>()
                .Join(moviesIds, e => e.MovieId, e => e, (movie, i) => new { movie, i })
                .Select(e => e.movie)
                .Include(e => e.Attachments)
                .Include(e => e.Studio)
                .AsQueryable();

            var page = await GetPageAsync(query, pagingFilter.OrderBy, pagingFilter.Page, pagingFilter.Size);

            return new Page<Movie>
            {
                Size = page.Size,
                Data = _mapper.Map<IEnumerable<Movie>>(page.Data),
                Offset = page.Offset,
                Total = page.Total
            };
        }

        public async Task<Movie> FindMovieAsync(int movieId)
        {
            var movie = await Context.Set<Binding.Models.Movie>()
                .Include(e => e.Studio)
                .Include(e => e.Attachments)
                .Include(e => e.MovieModels)
                    .ThenInclude(e => e.Model)
                .Include(e => e.MovieCategories)
                    .ThenInclude(e => e.Category)
                .FirstOrDefaultAsync(e => e.MovieId == movieId);

            return _mapper.Map<Movie>(movie);
        }

        public async Task<Movie> AddAsync(Movie movie)
        {
            var entity = _mapper.Map<Binding.Models.Movie>(movie);

            entity = Add(entity);

            await SaveChangesAsync();

            return _mapper.Map<Movie>(entity);
        }

        #region Add and Update

        public async Task<IEnumerable<Movie>> AddRangeAsync(IEnumerable<Movie> movies)
        {
            var source = movies as IList<Movie> ?? movies.ToList();

            var incommingModels = source.Where(e => e.Models != null && e.Models.Any()).SelectMany(e => e.Models);
            var existingModels = await SaveModelsAsync(incommingModels);

            var incommingCategories = source.Where(e => e.Categories != null && e.Categories.Any()).SelectMany(e => e.Categories);
            var existingCategories = await SaveCategoriesAsync(incommingCategories);

            var moviesToInsert = _mapper.Map<IList<Binding.Models.Movie>>(movies).ToList();

            MapModelsReferences(source, moviesToInsert, existingModels);
            MapCategoriesReferences(source, moviesToInsert, existingCategories);

            Context.Set<Binding.Models.Movie>().AddRange(moviesToInsert);

            await SaveChangesAsync();

            return _mapper.Map<IEnumerable<Movie>>(moviesToInsert);
        }

        public async Task UpdateAsync(IEnumerable<Movie> movies)
        {
            var source = movies as IList<Movie> ?? movies.ToList();

            var incommingModels = source.Where(e => e.Models != null && e.Models.Any()).SelectMany(e => e.Models);
            var existingModels = await SaveModelsAsync(incommingModels);

            var incommingCategories = source.Where(e => e.Categories != null && e.Categories.Any()).SelectMany(e => e.Categories);
            var existingCategories = await SaveCategoriesAsync(incommingCategories);

            var moviesToUpdate = Context.Set<Binding.Models.Movie>().Local.Where(e => source.Any(m => m.MovieId == e.MovieId)).ToList();

            if (!moviesToUpdate.Any())
            {
                var moviesToUpdateIds = source.Select(m => m.MovieId);
                moviesToUpdate = await Context.Set<Binding.Models.Movie>().Where(e => source.Any(m => moviesToUpdateIds.Contains(m.MovieId))).ToListAsync();
            }

            MapCategoriesReferences(source, moviesToUpdate, existingCategories);
            MapModelsReferences(source, moviesToUpdate, existingModels);

            Context.Set<Binding.Models.Movie>().UpdateRange(moviesToUpdate);

            await Context.SaveChangesAsync();
        }

        #endregion

        #region Private Methods

        private async Task<IList<Binding.Models.Category>> SaveCategoriesAsync(IEnumerable<Category> categories)
        {
            var existingCategories = await Context.Set<Binding.Models.Category>().ToListAsync();
            var newCategories = categories.Where(e => !existingCategories.Any(ex => string.Equals(ex.Name, e.Name, StringComparison.CurrentCultureIgnoreCase)));

            newCategories = newCategories
                .GroupBy(e => e.Name)
                .Select(group => group.First());

            if (newCategories.Any())
            {
                var categoriesToAdd = _mapper.Map<IEnumerable<Binding.Models.Category>>(newCategories).ToList();

                Context.Set<Binding.Models.Category>().AddRange(categoriesToAdd);
                await Context.SaveChangesAsync();

                Console.ForegroundColor = ConsoleColor.Magenta;

                foreach (var newCategory in categoriesToAdd)
                {
                    Console.WriteLine(newCategory.Name);
                }

                Console.ForegroundColor = ConsoleColor.White;

                existingCategories.AddRange(categoriesToAdd);
            }

            return existingCategories;
        }

        private async Task<IList<Binding.Models.Model>> SaveModelsAsync(IEnumerable<Model> models)
        {
            var existingModels = await Context.Set<Binding.Models.Model>().ToListAsync();

            var newModels = models.Where(e => !existingModels.Any(existing => CompareNames(e.Name, existing.Name))).ToList();

            newModels = newModels
                .GroupBy(e => e.Name)
                .Select(group => group.First()).ToList();

            if (newModels.Any())
            {
                var modelsToAdd = _mapper.Map<IEnumerable<Binding.Models.Model>>(newModels).ToList();

                Context.Set<Binding.Models.Model>().AddRange(modelsToAdd);
                await Context.SaveChangesAsync();

                Console.ForegroundColor = ConsoleColor.DarkYellow;

                foreach (var newModel in modelsToAdd)
                {
                    Console.WriteLine(newModel.Name);
                }

                Console.ForegroundColor = ConsoleColor.White;

                existingModels.AddRange(modelsToAdd);
            }

            return existingModels;
        }

        private void MapModelsReferences(IList<Movie> source, List<Binding.Models.Movie> target, IList<Binding.Models.Model> existingModels)
        {
            foreach (var targetMovie in target)
            {
                var movie = source.FirstOrDefault(e => e.Uri == targetMovie.Uri);

                if (movie != null)
                {
                    targetMovie.Description = movie.Description;
                }

                if (movie?.Models == null)
                {
                    continue;
                }

                var movieModels = _mapper.Map<IEnumerable<Binding.Models.Model>>(movie.Models).Select(e => new Binding.Models.MovieModel
                {
                    Model = new Binding.Models.Model
                    {
                        Name = e.Name
                    }
                });

                var skip = false;

                if (targetMovie.MovieModels != null && targetMovie.MovieModels.Any())
                {
                    var models = movieModels.Where(e => targetMovie.MovieModels.All(existing => !CompareNames(e.Model.Name, existing.Model.Name))).ToList();

                    if (models.Count == 0 && targetMovie.MovieId != 0)
                    {
                        skip = true;
                    }

                    foreach (var movieModel in models)
                    {
                        targetMovie.MovieModels.Add(movieModel);
                    }
                }
                else
                {
                    targetMovie.MovieModels = movieModels.ToList();
                }

                if (skip)
                {
                    continue;
                }

                foreach (var movieModel in targetMovie.MovieModels)
                {
                    var model = existingModels.FirstOrDefault(e => CompareNames(e.Name, movieModel.Model.Name));

                    if (model == null || movieModel.ModelId == model.ModelId)
                    {
                        continue;
                    }

                    movieModel.ModelId = model.ModelId;
                    movieModel.Model = null;
                }
            }
        }

        private void MapCategoriesReferences(IList<Movie> source, List<Binding.Models.Movie> target, IList<Binding.Models.Category> existingCategories)
        {
            foreach (var targetMovie in target)
            {
                var movie = source.FirstOrDefault(e => e.Uri == targetMovie.Uri);

                if (movie != null)
                {
                    targetMovie.Description = movie.Description;
                }

                if (movie?.Categories == null)
                {
                    continue;
                }

                var movieCategories = _mapper.Map<IEnumerable<Binding.Models.Category>>(movie.Categories).Select(e => new Binding.Models.MovieCategory
                {
                    Category = new Binding.Models.Category
                    {
                        Name = e.Name
                    }
                });

                var skip = false;

                if (targetMovie.MovieCategories != null && targetMovie.MovieCategories.Any())
                {
                    var categories = movieCategories.Where(e => targetMovie.MovieCategories.All(mc => mc.Category.Name != e.Category.Name)).ToList();

                    if (categories.Count == 0 && targetMovie.MovieId != 0)
                    {
                        skip = true;
                    }

                    foreach (var movieCategory in categories)
                    {
                        targetMovie.MovieCategories.Add(movieCategory);
                    }
                }
                else
                {
                    targetMovie.MovieCategories = movieCategories.ToList();
                }

                if (skip)
                {
                    continue;
                }

                foreach (var movieCategory in targetMovie.MovieCategories)
                {
                    var category = existingCategories.FirstOrDefault(e => string.Equals(e.Name, movieCategory.Category.Name, StringComparison.CurrentCultureIgnoreCase));

                    if (category == null || movieCategory.CategoryId == category.CategoryId)
                    {
                        continue;
                    }

                    movieCategory.CategoryId = category.CategoryId;
                    movieCategory.Category = null;
                }
            }
        }

        private static bool CompareNames(string name1, string name2)
        {
            var equal = name1.ToLower().Trim().Split().SequenceEqual(name2.ToLower().Trim().Split());

            return equal;
        }

        #endregion
    }
}
