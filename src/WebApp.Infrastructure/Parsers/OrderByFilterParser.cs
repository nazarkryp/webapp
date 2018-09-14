using System;
using System.Linq;

using WebApp.Infrastructure.Cache;
using WebApp.Infrastructure.Exceptions;
using WebApp.Infrastructure.Extensions;

namespace WebApp.Infrastructure.Parsers
{
    public class OrderByFilterParser : IOrderByFilterParser
    {
        private const string PropertyCacheKeyTemplate = "{0}-{1}-property-exists";

        private static ICacheStore _cacheStore;

        public OrderByFilterParser(ICacheStore cacheStore)
        {
            _cacheStore = cacheStore;
        }

        public string[] Parse<T>(string orderBy)
        {
            string[] orderByFilters = null;

            if (orderBy != null)
            {
                orderByFilters = orderBy.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var invalidFilters = orderByFilters.Where(filter => !FilterExists<T>(filter)).ToList();
                if (invalidFilters.Any())
                {
                    throw new InvalidFilterCriteriaException(invalidFilters);
                }
            }

            return orderByFilters;
        }

        private static bool FilterExists<T>(string filter)
        {
            var type = typeof(T);

            var clearedFilter = RemoveFilteringCharacters(filter);
            var key = string.Format(PropertyCacheKeyTemplate, type.Name, clearedFilter);
            var propertyExists = _cacheStore.GetItem<bool?>(key);

            if (!propertyExists.HasValue)
            {
                propertyExists = type.PropertyExists(clearedFilter);

                _cacheStore.SetItem(key, propertyExists);
            }

            return propertyExists.Value;
        }

        private static string RemoveFilteringCharacters(string str)
        {
            var result = str;

            if (str.StartsWith("+"))
            {
                result = str.TrimStart('+');
            }

            if (str.StartsWith("-"))
            {
                result = str.TrimStart('-');
            }

            return result;
        }
    }
}
