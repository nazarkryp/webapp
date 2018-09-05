using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using WebApp.Repositories.EntityFramework.Context;

namespace WebApp.Repositories.EntityFramework.Repositories
{
    internal class GenericRepository<TEntity> where TEntity : class
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

        public Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().FirstOrDefaultAsync(predicate);
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
    }
}
