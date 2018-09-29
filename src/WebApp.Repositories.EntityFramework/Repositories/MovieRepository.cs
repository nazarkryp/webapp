﻿using System;
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
                .Where(e => e.StudioId == studioId && !e.MovieCategories.Any()).OrderByDescending(e => e.MovieId).ToListAsync();

            return _mapper.Map<IEnumerable<Movie>>(movies);
        }

        public async Task<Page<Movie>> GetPageAsync(MoviesPagingFilter pagingFilter)
        {
            IQueryable<Binding.Models.Movie> query = Context.Set<Binding.Models.Movie>();

            if (!string.IsNullOrEmpty(pagingFilter?.SearchQuery))
            {
                query = query.Where(e => e.Title.ToLower().Contains(pagingFilter.SearchQuery) || e.Description.ToLower().Contains(pagingFilter.SearchQuery));
            }

            if (pagingFilter?.Categories != null && pagingFilter?.Categories.Length > 0)
            {
                var categoriesNames = pagingFilter.Categories.Select(e => e.ToLower()).ToArray();
                var categories = await Context.Set<Binding.Models.Category>().Where(e => categoriesNames.Contains(e.Name.ToLower())).ToListAsync();

                if (categories == null || !categories.Any())
                {
                    return new Page<Movie>();
                }

                var categoriesIds = categories.Select(e => e.CategoryId).ToArray();
                query = query.Where(e => e.MovieCategories.Count(c => categoriesIds.Contains(c.CategoryId)) == categoriesIds.Length);
            }

            if (pagingFilter?.StudioIds != null && pagingFilter.StudioIds.Length > 0)
            {
                query = query.Where(e => pagingFilter.StudioIds.Distinct().Contains(e.StudioId));
            }

            //query = query
            //    .Include(e => e.Studio)
            //    .Include(e => e.Attachments)
            //    .Include(e => e.MovieModels)
            //    .ThenInclude(e => e.Model);

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

        public async Task<IEnumerable<Movie>> AddRangeAsync(IEnumerable<Movie> movies)
        {
            var source = movies as IList<Movie> ?? movies.ToList();

            var incommingModels = source.Where(e => e.Models != null && e.Models.Any()).SelectMany(e => e.Models);
            var existingModels = await SaveModelsAsync(incommingModels);

            var incommingCategories = source.Where(e => e.Categories != null && e.Categories.Any()).SelectMany(e => e.Categories);
            var existingCategories = await SaveCategoriesAsync(incommingCategories);

            //var existingModels = await Context.Set<Binding.Models.Model>().ToListAsync();

            //var models = source.SelectMany(e => e.Models).Where(e => existingModels.All(existing => !CompareNames(e.Name, existing.Name))).ToList();

            //var modelsToSave = _mapper.Map<IEnumerable<Binding.Models.Model>>(models).ToList();

            //if (modelsToSave.Any())
            //{
            //    Context.Set<Binding.Models.Model>().AddRange(modelsToSave);

            //    await SaveChangesAsync();

            //    existingModels = await Context.Set<Binding.Models.Model>().ToListAsync();
            //}

            var moviesToInsert = _mapper.Map<IList<Binding.Models.Movie>>(movies).ToList();

            MapModelsReferences(source, moviesToInsert, existingModels);
            MapCategoriesReferences(source, moviesToInsert, existingCategories);

            await Context.Set<Binding.Models.Movie>().AddRangeAsync(moviesToInsert);

            await SaveChangesAsync();

            return _mapper.Map<IEnumerable<Movie>>(moviesToInsert);
        }

        public async Task<Movie> UpdateAsync(Movie movie)
        {
            var existingCategories = await SaveCategoriesAsync(movie.Categories);

            var movieToUpdate = Context.Set<Binding.Models.Movie>().Local.FirstOrDefault(e => e.MovieId == movie.MovieId)
                                ?? await Context.Set<Binding.Models.Movie>().FirstOrDefaultAsync(e => e.MovieId == movie.MovieId);

            _mapper.Map(movie, movieToUpdate);

            foreach (var movieCategory in movieToUpdate.MovieCategories)
            {
                var category = existingCategories.FirstOrDefault(e => string.Equals(e.Name, movieCategory.Category.Name, StringComparison.CurrentCultureIgnoreCase));

                if (category == null)
                {
                    continue;
                }

                movieCategory.CategoryId = category.CategoryId;
                movieCategory.Category = null;
            }

            Context.Set<Binding.Models.Movie>().Update(movieToUpdate);

            await Context.SaveChangesAsync();

            return movie;
        }

        public async Task UpdateAsync(IEnumerable<Movie> movies)
        {
            var source = movies as IList<Movie> ?? movies.ToList();

            var incommingModels = source.Where(e => e.Models != null && e.Models.Any()).SelectMany(e => e.Models);
            var existingModels = await SaveModelsAsync(incommingModels);

            var incommingCategories = source.Where(e => e.Categories != null && e.Categories.Any()).SelectMany(e => e.Categories);
            var existingCategories = await SaveCategoriesAsync(incommingCategories);

            var moviesToUpdate = Context.Set<Binding.Models.Movie>().Local.Where(e => source.Any(m => m.MovieId == e.MovieId)).ToList();

            MapCategoriesReferences(source, moviesToUpdate, existingCategories);
            MapModelsReferences(source, moviesToUpdate, existingModels);

            Context.Set<Binding.Models.Movie>().UpdateRange(moviesToUpdate);

            await Context.SaveChangesAsync();
        }

        private async Task<IList<Binding.Models.Category>> SaveCategoriesAsync(IEnumerable<Category> categories)
        {
            var existingCategories = await Context.Set<Binding.Models.Category>().ToListAsync();
            var newCategories = categories.Where(e => !existingCategories.Any(ex => string.Equals(ex.Name, e.Name, StringComparison.CurrentCultureIgnoreCase)));

            if (newCategories.Any())
            {
                var categoriesToAdd = _mapper.Map<IEnumerable<Binding.Models.Category>>(newCategories).ToList();

                await Context.Set<Binding.Models.Category>().AddRangeAsync(categoriesToAdd);
                await Context.SaveChangesAsync();

                Console.ForegroundColor = ConsoleColor.DarkYellow;

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

            var newModels = models.Where(e => existingModels.All(existing => !CompareNames(e.Name, existing.Name)));

            if (newModels.Any())
            {
                var modelsToAdd = _mapper.Map<IEnumerable<Binding.Models.Model>>(newModels).ToList();

                await Context.Set<Binding.Models.Model>().AddRangeAsync(modelsToAdd);
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

                    if (models.Count == 0)
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
                    var model = existingModels.FirstOrDefault(e => string.Equals(e.Name, movieModel.Model.Name, StringComparison.CurrentCultureIgnoreCase));

                    if (model == null || movieModel.ModelId == model.ModelId)
                    {
                        continue;
                    }

                    movieModel.ModelId = model.ModelId;
                    movieModel.Model = null;
                }
            }

            //var entityMovieModels = entities.SelectMany(e => e.MovieModels);

            //foreach (var entityMovieModel in entityMovieModels)
            //{
            //    var existingModel = existingModels.FirstOrDefault(existing => existing.Name.ToLower().Split().All(e => entityMovieModel.Model.Name.ToLower().Split().Any(n => n == e)));

            //    if (existingModel == null)
            //    {
            //        continue;
            //    }

            //    entityMovieModel.ModelId = existingModel.ModelId;
            //    entityMovieModel.Model = null;
            //}
        }

        private void MapCategoriesReferences(IList<Movie> source, List<Binding.Models.Movie> target, IList<Binding.Models.Category> existingCategories)
        {
            foreach (var targetMovie in target)
            {
                var movie = source.FirstOrDefault(e => e.Uri == targetMovie.Uri);

                //_mapper.Map(movie, movieToUpdate);

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

                    if (categories.Count == 0)
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
            return name1.ToLower().Split().SequenceEqual(name2.ToLower().Split());
        }
    }
}
