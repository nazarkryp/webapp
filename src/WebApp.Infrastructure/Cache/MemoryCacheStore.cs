using System;
using System.Linq;
using System.Runtime.Caching;

namespace WebApp.Infrastructure.Cache
{
    public class MemoryCacheStore : ICacheStore
    {
        private readonly ObjectCache _memoryCache = MemoryCache.Default;

        public T GetItem<T>(string key)
        {
            var item = (T)_memoryCache.Get(key);

            return item;
        }

        public void SetItem(string key, object value)
        {
            _memoryCache[key] = value;
        }

        public void SetItem(string key, object value, DateTime expirationTime)
        {
            var cachePolicy = new CacheItemPolicy()
            {
                AbsoluteExpiration = new DateTimeOffset(expirationTime)
            };

            _memoryCache.Set(key, value, cachePolicy);
        }

        public void AddItem(string key, object value, TimeSpan expirationTime)
        {
            var cachePolicy = new CacheItemPolicy()
            {
                SlidingExpiration = expirationTime
            };

            _memoryCache.Add(key, value, cachePolicy);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        public void RemoveAll<T>()
        {
            var cacheKeys = _memoryCache.Where(kvp => kvp.Value is T).Select(kvp => kvp.Key).ToList();

            foreach (var cacheKey in cacheKeys)
            {
                _memoryCache.Remove(cacheKey);
            }
        }
    }
}
