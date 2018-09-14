using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using WebApp.Repositories.Common;

namespace WebApp.Repositories.EntityFramework.Extensions
{
    internal static class QueryableExtensions
    {
        public static async Task<Page<TEntity>> GetPageAsync<TEntity>(
             this IQueryable<TEntity> source,
             string orderBy,
             int offset,
             int size) where TEntity : class
        {
            if (string.IsNullOrEmpty(orderBy))
            {
                throw new ArgumentException(nameof(orderBy));
            }

            if (size < 0)
            {
                throw new ArgumentException(nameof(size));
            }

            var total = source.Count();
            var query = source.OrderByDescending(e => e).Skip(offset).Take(size);

            var filtered = query is IAsyncEnumerable<TEntity> ? await query.ToListAsync() : query.ToList();

            return new Page<TEntity>
            {
                Total = total,
                Data = filtered,
                Size = size,
                Offset = offset
            };
        }
    }
}
