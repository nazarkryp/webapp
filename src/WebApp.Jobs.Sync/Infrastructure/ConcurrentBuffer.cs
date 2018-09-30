using System.Collections.Concurrent;

namespace WebApp.Jobs.Sync.Infrastructure
{
    public class ConcurrentBuffer<T> : ConcurrentBag<T>
    {
        public new void Add(T item)
        {
            base.Add(item);
        }

        public new void Clear()
        {
            base.Clear();
        }
    }
}
