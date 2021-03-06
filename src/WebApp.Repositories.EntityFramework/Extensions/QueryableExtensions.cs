﻿using System;
using System.Linq;
using System.Linq.Expressions;
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
             int page,
             int size)
        {
            if (string.IsNullOrEmpty(orderBy))
            {
                throw new ArgumentException(nameof(orderBy));
            }

            if (size < 1)
            {
                throw new ArgumentException(nameof(size));
            }

            if (page < 1)
            {
                throw new ArgumentException(nameof(page));
            }

            var total = await source.CountAsync();
            var filtered = await source.OrderBy(orderBy).Skip((page - 1) * size).Take(size).ToListAsync();

            return new Page<TEntity>
            {
                Total = total,
                Data = filtered,
                Size = size,
                Offset = page
            };
        }

        private static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering)
        {
            var type = typeof(T);
            var property = type.GetProperty(ordering);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            var resultExp = Expression.Call(typeof(Queryable), "OrderByDescending", new Type[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExp));

            return source.Provider.CreateQuery<T>(resultExp);
        }
    }
}
