using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApp.Infrastructure.Handlers
{
    public interface IConcurrentActionHandler
    {
        Task ForAsync(Func<int, Task> action, int fromInclusive, int toExclusive, int maxDegreeOfParallelism);

        Task ForeachAsync<TSource>(IEnumerable<TSource> source, Func<TSource, Task> action, int maxDegreeOfParallelism);

        Task ForeachAsync<TSource>(IEnumerable<TSource> source, Func<TSource, Task<TSource>> action,
            int maxDegreeOfParallelism);
    }
}
