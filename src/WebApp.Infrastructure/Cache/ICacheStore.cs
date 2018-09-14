using System;

namespace WebApp.Infrastructure.Cache
{
    public interface ICacheStore
    {
        T GetItem<T>(string key);

        void SetItem(string key, object value);

        void SetItem(string key, object value, DateTime expirationTime);

        void AddItem(string key, object value, TimeSpan expirationTime);

        void Remove(string key);

        void RemoveAll<T>();
    }
}
