using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using WebApp.Repositories.Common;
using WebApp.Repositories.EntityFramework.Context;
using WebApp.Repositories.EntityFramework.Extensions;

namespace WebApp.Repositories.EntityFramework.Repositories
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        protected readonly IDbContext Context;

        public GenericRepository(IDbContext context)
        {
            Context = context;
        }

        public IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate = null)
        {
            return predicate != null ? Context.Set<TEntity>().Where(predicate) : Context.Set<TEntity>();
        }

        public IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate = null, params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            var query = BuildQuery(navigationProperties);

            return predicate != null ? query.Where(predicate) : query;
        }

        public Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }

        public Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            var query = BuildQuery(navigationProperties);

            return query.FirstOrDefaultAsync(predicate);
        }

        public TEntity Add(TEntity entity)
        {
            return Context.Set<TEntity>().Add(entity).Entity;
        }

        public void RemoveRange(IQueryable<TEntity> entities)
        {
            Context.Set<TEntity>().RemoveRange(entities);
        }

        public Task<int> SaveChangesAsync()
        {
            return Context.SaveChangesAsync();
        }

        private IQueryable<TEntity> BuildQuery(Expression<Func<TEntity, object>>[] navigationProperties)
        {
            IQueryable<TEntity> query = Context.Set<TEntity>();

            if (navigationProperties?.Any() != null)
            {
                query = navigationProperties.Aggregate(query, (current, property) => current.Include(property));
            }

            return query;
        }

        protected async Task<Page<T>> GetPageAsync<T>(IQueryable<T> query, string[] orderByFields, int? next, int? size) where T : class
        {
            if (!size.HasValue)
            {
                size = 12;
            }

            if (size <= 0)
            {
                size = 12;
            }
            else if (size > 12)
            {
                size = 12;
            }

            var orderByFilter = BuildOrderByFilter(orderByFields);

            return await query.GetPageAsync(orderByFilter, next ?? 0, size.Value);
        }

        private static string BuildOrderByFilter(string[] orderByFields)
        {
            if (orderByFields == null)
            {
                return string.Empty;
            }

            var sortFields = new List<string>();

            foreach (string orderField in orderByFields)
            {
                if (orderField.StartsWith("+"))
                {
                    sortFields.Add($"{orderField.TrimStart('+')} ASC");
                }
                else if (orderField.StartsWith("-"))
                {
                    sortFields.Add($"{orderField.TrimStart('-')} DESC");
                }
                else
                {
                    sortFields.Add(orderField);
                }
            }

            return string.Join(",", sortFields);
        }
    }
}
