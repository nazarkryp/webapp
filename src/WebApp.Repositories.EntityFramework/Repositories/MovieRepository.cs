﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        #region Fields

        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public MovieRepository(IDbContext context, IMapper mapper)
            : base(context)
        {
            _mapper = mapper;
        }

        #endregion

        #region Job Methods

        public async Task<IEnumerable<Movie>> FindLatestAsync(int studioId)
        {
            //var max = await Context.Set<Binding.Models.Movie>().Where(e => e.StudioId == studioId).MaxAsync(e => e.Date);
            var last = await Context.Set<Binding.Models.Movie>().Where(e => e.StudioId == studioId).OrderByDescending(e => e.Date).FirstOrDefaultAsync();
            var max = last.Date;
            var movies = await Context.Set<Binding.Models.Movie>().Where(e => e.StudioId == studioId && e.Date == max).ToListAsync();

            return _mapper.Map<IEnumerable<Movie>>(movies);
        }

        public async Task<Movie> FindLastAsync(int studioId)
        {
            var movie = await Context.Set<Binding.Models.Movie>().OrderByDescending(e => e.Date).FirstOrDefaultAsync();

            return _mapper.Map<Movie>(movie);
        }

        public async Task<IEnumerable<Movie>> FindMoviesWithoutDetailsAsync(int studioId)
        {
            var movies = await Context.Set<Binding.Models.Movie>()
                .Where(e => e.StudioId == studioId && e.Description == null)
                .OrderByDescending(e => e.MovieId)
                .ToListAsync();

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

        #endregion

        #region Find Movies

        public async Task<Page<Movie>> FindMoviesAsync(MoviesPagingFilter pagingFilter)
        {
            var watch = Stopwatch.StartNew();

            IQueryable<Binding.Models.Movie> movies = null;

            if (pagingFilter.Models?.Length >= 1)
            {
                var lowerModelsNames = pagingFilter.Models.Select(e => e.ToLower()).ToArray();
                var ids = await Context.Set<Binding.Models.Model>().Where(e => lowerModelsNames.Contains(e.Name.ToLower())).Select(e => e.ModelId).ToListAsync();
                var idsLength = ids.Count;

                if (idsLength == 0)
                {
                    return new Page<Movie>();
                }

                movies = Context.Set<Binding.Models.Movie>()
                    .Where(e => e.MovieModels.Count(x => ids.Contains(x.ModelId)) == idsLength);
            }

            if (pagingFilter.Categories?.Length >= 1)
            {
                var lowerCategoriesNames = pagingFilter.Categories.Select(e => e.ToLower()).ToArray();
                var ids = await Context.Set<Binding.Models.Category>().Where(e => lowerCategoriesNames.Contains(e.Name.ToLower())).Select(e => e.CategoryId).ToListAsync();
                var idsLength = ids.Count;

                if (idsLength == 0)
                {
                    return new Page<Movie>();
                }

                movies = (movies ?? Context.Set<Binding.Models.Movie>())
                    .Where(e => e.MovieCategories.Count(x => ids.Contains(x.CategoryId)) == idsLength);
            }

            if (pagingFilter.Studios?.Length >= 1)
            {
                movies = (movies ?? Context.Set<Binding.Models.Movie>())
                    .Where(e => pagingFilter.Studios.Contains(e.StudioId));
            }

            if (movies == null)
            {
                movies = Context.Set<Binding.Models.Movie>();
            }

            if (pagingFilter.Date.HasValue)
            {
                movies = movies
                    .Where(e => e.Date == pagingFilter.Date);
            }

            movies = movies
                .Include(e => e.Attachments)
                .Include(e => e.Studio);

            var page = await GetPageAsync(movies, pagingFilter.OrderBy, pagingFilter.Page, pagingFilter.Size);

            watch.Stop();
            var seconds = watch.Elapsed.Seconds;

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

        #endregion

        #region Add and Update

        public async Task<IEnumerable<Movie>> AddRangeAsync(IEnumerable<Movie> movies)
        {
            var source = movies as IList<Movie> ?? movies.ToList();

            var invalid = source.Where(e => e.Studio == null || e.Studio.StudioId < 1);

            var incommingModels = source.Where(e => e.Models != null && e.Models.Any()).SelectMany(e => e.Models);
            var existingModels = await SaveModelsAsync(incommingModels);

            var incommingCategories = source.Where(e => e.Categories != null && e.Categories.Any()).SelectMany(e => e.Categories);
            var existingCategories = await SaveCategoriesAsync(incommingCategories);

            var moviesToInsert = _mapper.Map<IList<Binding.Models.Movie>>(movies).ToList();

            await MapMoviesFromSourceAsync(source, moviesToInsert, existingCategories, existingModels);

            Context.Set<Binding.Models.Movie>().AddRange(moviesToInsert);

            await SaveChangesAsync();

            return _mapper.Map<IEnumerable<Movie>>(moviesToInsert);
        }

        public async Task UpdateAsync(IEnumerable<Movie> movies)
        {
            var source = movies as IList<Movie> ?? movies.ToList();

            var incommingCategories = source.Where(e => e.Categories != null && e.Categories.Any()).SelectMany(e => e.Categories);
            var existingCategories = await SaveCategoriesAsync(incommingCategories);
            var incommingModels = source.Where(e => e.Models != null && e.Models.Any()).SelectMany(e => e.Models);
            var existingModels = await SaveModelsAsync(incommingModels);

            //var moviesToUpdate = Context.Set<Binding.Models.Movie>().Local.Where(e => source.Any(m => m.MovieId == e.MovieId)).ToList();

            //if (!moviesToUpdate.Any())
            //{
            //    var moviesToUpdateIds = source.Select(m => m.MovieId);
            //    moviesToUpdate = await Context.Set<Binding.Models.Movie>().Where(e => source.Any(m => moviesToUpdateIds.Contains(m.MovieId))).ToListAsync();
            //}

            var moviesToUpdateIds = source.Select(m => m.MovieId).ToArray();
            //var moviesToUpdate = await Context.Set<Binding.Models.Movie>().Where(e => source.Any(m => moviesToUpdateIds.Contains(m.MovieId))).ToListAsync();
            var moviesToUpdate = await Context.Set<Binding.Models.Movie>()
                .Where(e => moviesToUpdateIds.Contains(e.MovieId)).ToListAsync();

            await MapMoviesFromSourceAsync(source, moviesToUpdate, existingCategories, existingModels);
            Context.Set<Binding.Models.Movie>().UpdateRange(moviesToUpdate);

            await Context.SaveChangesAsync();
        }

        #endregion

        #region Private Methods

        private async Task<IList<Binding.Models.Category>> SaveCategoriesAsync(IEnumerable<Category> categories)
        {
            var existingCategories = await Context.Set<Binding.Models.Category>().ToListAsync();
            var newCategories = categories.Where(e => !existingCategories.Any(ex => string.Equals(ex.Name, e.Name.Trim(), StringComparison.CurrentCultureIgnoreCase)));

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

            // Distinct() via GroupBy
            newModels = newModels.GroupBy(e => e.Name).Select(group => group.First()).ToList();

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

        #endregion

        #region Mapping Helpers

        private async Task MapMoviesFromSourceAsync(IList<Movie> source, List<Binding.Models.Movie> moviesToUpdate, IList<Binding.Models.Category> existingCategories, IList<Binding.Models.Model> existingModels)
        {
            var sourceMoviesIds = source.Where(e => e.MovieId != 0).Select(e => e.MovieId);

            IList<Binding.Models.MovieModel> existingMovieModels = await Context.Set<Binding.Models.MovieModel>().Where(e => sourceMoviesIds.Contains(e.MovieId)).ToListAsync();
            IList<Binding.Models.MovieCategory> existingMovieCategories = await Context.Set<Binding.Models.MovieCategory>().Where(e => sourceMoviesIds.Contains(e.MovieId)).ToListAsync();
            IList<Binding.Models.Attachment> existingMovieAttachments = await Context.Set<Binding.Models.Attachment>().Where(e => sourceMoviesIds.Contains(e.MovieId)).ToListAsync();

            foreach (var sourceMovie in source)
            {
                var targetMovie = moviesToUpdate.FirstOrDefault(e => e.Uri == sourceMovie.Uri || (sourceMovie.MovieId != 0 && e.MovieId != 0 && e.MovieId == sourceMovie.MovieId));

                if (targetMovie != null)
                {
                    var attachments = sourceMovie.Attachments.Where(e => existingMovieAttachments.All(a => a.Uri != e.Uri));
                    sourceMovie.Attachments = attachments.ToList();
                    targetMovie = _mapper.Map(sourceMovie, targetMovie);

                    //MapAttachments(sourceMovie, targetMovie, existingMovieAttachments);
                    MapCategoriesReferences(sourceMovie, targetMovie, existingMovieCategories, existingCategories);
                    MapModelsReferences(sourceMovie, targetMovie, existingMovieModels, existingModels);
                }
            }
        }

        private static void MapAttachments(Movie source, Binding.Models.Movie targetMovie, IList<Binding.Models.Attachment> existingMovieAttachments)
        {
            //var movieAttachments = existingMovieAttachments?.Where(e => e.MovieId == targetMovie.MovieId).ToList();

            //if (movieAttachments != null && movieAttachments.Any())
            //{
            //    var attachmentsToRemove = targetMovie.Attachments.Where(e => movieAttachments.Any(ma => ma.AttachmentId == e.AttachmentId));

            //    foreach (var attachment in attachmentsToRemove)
            //    {
            //        targetMovie.Attachments.Remove(attachment);
            //    }
            //}
        }

        private static void MapCategoriesReferences(Movie source, Binding.Models.Movie targetMovie, IList<Binding.Models.MovieCategory> existingMovieCategories, IList<Binding.Models.Category> existingCategories)
        {
            if (source?.Categories == null)
            {
                return;
            }

            foreach (var movieCategory in targetMovie.MovieCategories)
            {
                var category = existingCategories.FirstOrDefault(e => string.Equals(e.Name, movieCategory.Category.Name, StringComparison.CurrentCultureIgnoreCase));

                var existingMovieCategory = existingMovieCategories.FirstOrDefault(e => e.MovieId == targetMovie.MovieId && e.CategoryId == category.CategoryId);
                if (existingMovieCategory != null)
                {
                    movieCategory.CategoryId = existingMovieCategory.CategoryId;
                    movieCategory.MovieId = existingMovieCategory.MovieId;
                    movieCategory.Category = null;
                    continue;
                }

                if (category == null || movieCategory.CategoryId == category.CategoryId)
                {
                    continue;
                }

                movieCategory.CategoryId = category.CategoryId;
                movieCategory.Category = null;
            }
        }

        private static void MapModelsReferences(Movie source, Binding.Models.Movie targetMovie, IList<Binding.Models.MovieModel> existingMovieModels, IList<Binding.Models.Model> existingModels)
        {
            if (source?.Models == null)
            {
                return;
            }

            foreach (var movieModel in targetMovie.MovieModels)
            {
                var model = existingModels.FirstOrDefault(e => CompareNames(e.Name, movieModel.Model.Name));

                var existingMovieModel = existingMovieModels.FirstOrDefault(e => e.MovieId == targetMovie.MovieId && e.ModelId == model.ModelId);
                if (existingMovieModel != null)
                {
                    movieModel.ModelId = existingMovieModel.ModelId;
                    movieModel.MovieId = existingMovieModel.MovieId;
                    movieModel.Model = null;
                    continue;
                }

                if (model == null || movieModel.ModelId == model.ModelId)
                {
                    continue;
                }

                movieModel.ModelId = model.ModelId;
                movieModel.Model = null;
            }
        }

        private static bool CompareNames(string name1, string name2)
        {
            var equal = name1.ToLower().Trim().Split().SequenceEqual(name2.ToLower().Trim().Split());

            return equal;
        }

        //private void MapModelsReferences(IList<Movie> source, List<Binding.Models.Movie> target, IList<Binding.Models.Model> existingModels)
        //{
        //    foreach (var targetMovie in target)
        //    {
        //        var movie = source.FirstOrDefault(e => e.Uri == targetMovie.Uri);

        //        if (movie?.Models == null)
        //        {
        //            continue;
        //        }

        //        var movieModels = _mapper.Map<IEnumerable<Binding.Models.Model>>(movie.Models).Select(e => new Binding.Models.MovieModel
        //        {
        //            Model = new Binding.Models.Model
        //            {
        //                Name = e.Name
        //            }
        //        });

        //        var skip = false;

        //        if (targetMovie.MovieModels != null && targetMovie.MovieModels.Any())
        //        {
        //            var models = movieModels.Where(e => targetMovie.MovieModels.All(existing => !CompareNames(e.Model.Name, existing.Model.Name))).ToList();

        //            if (models.Count == 0 && targetMovie.MovieId != 0)
        //            {
        //                skip = true;
        //            }

        //            foreach (var movieModel in models)
        //            {
        //                targetMovie.MovieModels.Add(movieModel);
        //            }
        //        }
        //        else
        //        {
        //            targetMovie.MovieModels = movieModels.ToList();
        //        }

        //        if (skip)
        //        {
        //            continue;
        //        }

        //        foreach (var movieModel in targetMovie.MovieModels)
        //        {
        //            var model = existingModels.FirstOrDefault(e => CompareNames(e.Name, movieModel.Model.Name));

        //            if (model == null || movieModel.ModelId == model.ModelId)
        //            {
        //                continue;
        //            }

        //            movieModel.ModelId = model.ModelId;
        //            movieModel.Model = null;
        //        }
        //    }
        //}

        //private void MapCategoriesReferences(IList<Movie> source, List<Binding.Models.Movie> target, IList<Binding.Models.Category> existingCategories)
        //{
        //    foreach (var targetMovie in target)
        //    {
        //        var movie = source.FirstOrDefault(e => e.Uri == targetMovie.Uri);

        //        if (movie?.Categories == null)
        //        {
        //            continue;
        //        }

        //        var movieCategories = _mapper.Map<IEnumerable<Binding.Models.Category>>(movie.Categories).Select(e => new Binding.Models.MovieCategory
        //        {
        //            Category = new Binding.Models.Category
        //            {
        //                Name = e.Name
        //            }
        //        });

        //        var skip = false;

        //        if (targetMovie.MovieCategories != null && targetMovie.MovieCategories.Any())
        //        {
        //            var categories = movieCategories.Where(e => !targetMovie.MovieCategories.Any(mc => string.Equals(mc.Category.Name, e.Category.Name, StringComparison.CurrentCultureIgnoreCase))).ToList();

        //            if (categories.Count == 0 && targetMovie.MovieId != 0)
        //            {
        //                skip = true;
        //            }

        //            foreach (var movieCategory in categories)
        //            {
        //                targetMovie.MovieCategories.Add(movieCategory);
        //            }
        //        }
        //        else
        //        {
        //            targetMovie.MovieCategories = movieCategories.ToList();
        //        }

        //        if (skip)
        //        {
        //            continue;
        //        }

        //        var c = targetMovie.MovieCategories.FirstOrDefault(e => e.Category.Name == "Ass licking");

        //        foreach (var movieCategory in targetMovie.MovieCategories)
        //        {
        //            var category = existingCategories.FirstOrDefault(e => string.Equals(e.Name, movieCategory.Category.Name, StringComparison.CurrentCultureIgnoreCase));

        //            if (category == null || movieCategory.CategoryId == category.CategoryId)
        //            {
        //                continue;
        //            }

        //            movieCategory.CategoryId = category.CategoryId;
        //            movieCategory.Category = null;
        //        }
        //    }
        //}

        #endregion
    }
}
