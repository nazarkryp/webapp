﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Remotion.Linq.Utilities;
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

        public async Task<Page<Movie>> GetPageAsync(MoviesPagingFilter pagingFilter)
        {
            IQueryable<Binding.Models.Movie> query = Context.Set<Binding.Models.Movie>();

            if (!string.IsNullOrEmpty(pagingFilter?.SearchQuery))
            {
                query = query.Where(e => e.Title.ToLower().Contains(pagingFilter.SearchQuery) || e.Description.ToLower().Contains(pagingFilter.SearchQuery));
            }

            if (pagingFilter?.StudioIds != null && pagingFilter.StudioIds.Length > 0)
            {
                query = query.Where(e => pagingFilter.StudioIds.Distinct().Contains(e.StudioId));
            }

            query = query
                .Include(e => e.Studio)
                .Include(e => e.Attachments)
                .Include(e => e.MovieModels)
                .ThenInclude(e => e.Model);

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

        public async Task<Movie> UpdateAsync(Movie movie)
        {
            var existingCategories = await Context.Set<Binding.Models.Category>().ToListAsync();
            var newCategories = movie.Categories.Where(e => !existingCategories.Any(ex => string.Equals(ex.Name, e.Name, StringComparison.CurrentCultureIgnoreCase)));

            if (newCategories.Any())
            {
                var categoriesToAdd = _mapper.Map<IEnumerable<Binding.Models.Category>>(newCategories).ToList();

                await Context.Set<Binding.Models.Category>().AddRangeAsync(categoriesToAdd);
                await Context.SaveChangesAsync();

                existingCategories.AddRange(categoriesToAdd);
            }

            var movieToUpdate = Context.Set<Binding.Models.Movie>().Local.FirstOrDefault(e => e.MovieId == movie.MovieId);

            _mapper.Map(movie, movieToUpdate);

            foreach (var movieCategory in movieToUpdate.MovieCategories)
            {
                movieCategory.CategoryId = existingCategories.FirstOrDefault(e => string.Equals(e.Name, movieCategory.Category.Name, StringComparison.CurrentCultureIgnoreCase)).CategoryId;
                movieCategory.Category = null;
            }

            Context.Set<Binding.Models.Movie>().Update(movieToUpdate);

            await Context.SaveChangesAsync();

            return movie;
        }

        public async Task UpdateAsync(IEnumerable<Movie> movies)
        {
            var movieList = movies as IList<Movie> ?? movies.ToList();

            var incommingCategories = movieList.Where(e => e.Categories != null && e.Categories.Any()).SelectMany(e => e.Categories);
            var existingCategories = await SaveCategoriesAsync(incommingCategories);

            var moviesToUpdate = Context.Set<Binding.Models.Movie>().Local.Where(e => movieList.Any(m => m.MovieId == e.MovieId)).ToList();

            MapCategoriesReferences(movieList, moviesToUpdate, existingCategories);

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

                existingCategories.AddRange(categoriesToAdd);
            }

            return existingCategories;
        }

        public async Task<IEnumerable<Movie>> AddRangeAsync(IEnumerable<Movie> movies)
        {
            var existingModels = await Context.Set<Binding.Models.Model>().ToListAsync();

            var models = movies.SelectMany(e => e.Models).Where(e => existingModels.All(existing => !CompareNames(e.Name, existing.Name))).ToList();

            var modelsToSave = _mapper.Map<IEnumerable<Binding.Models.Model>>(models);

            if (modelsToSave.Any())
            {
                Context.Set<Binding.Models.Model>().AddRange(modelsToSave);

                await SaveChangesAsync();

                existingModels = await Context.Set<Binding.Models.Model>().ToListAsync();
            }

            var entities = _mapper.Map<IList<Binding.Models.Movie>>(movies);

            MapReferences(existingModels, entities);

            await Context.Set<Binding.Models.Movie>().AddRangeAsync(entities);

            await SaveChangesAsync();

            return _mapper.Map<IEnumerable<Movie>>(entities);
        }

        public async Task<IEnumerable<Movie>> FindAllStudioMoviesAsync(int studioId)
        {
            var movies = await Context.Set<Binding.Models.Movie>()
                .Where(e => e.StudioId == studioId && !e.MovieCategories.Any()).OrderByDescending(e => e.MovieId).ToListAsync();

            return _mapper.Map<IEnumerable<Movie>>(movies);
        }

        private static void MapReferences(IReadOnlyCollection<Binding.Models.Model> existingModels, IEnumerable<Binding.Models.Movie> entities)
        {
            var entityMovieModels = entities.SelectMany(e => e.MovieModels);

            foreach (var entityMovieModel in entityMovieModels)
            {
                var existingModel = existingModels.FirstOrDefault(existing => existing.Name.ToLower().Split().All(e => entityMovieModel.Model.Name.ToLower().Split().Any(n => n == e)));

                if (existingModel == null)
                {
                    continue;
                }

                entityMovieModel.ModelId = existingModel.ModelId;
                entityMovieModel.Model = null;
            }
        }

        private void MapCategoriesReferences(IList<Movie> source, List<Binding.Models.Movie> target, IList<Binding.Models.Category> existingCategories)
        {
            foreach (var movieToUpdate in target)
            {
                var movie = source.FirstOrDefault(e => e.MovieId == movieToUpdate.MovieId);

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
            }
        }

        private static bool CompareNames(string name1, string name2)
        {
            return name1.ToLower().Split().SequenceEqual(name2.ToLower().Split());
        }
    }
}
