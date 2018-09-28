using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WebApp.Infrastructure.Handlers
{
    public interface IConcurrentActionHandler
    {
        Task ForAsync(Func<int, Task> action, int fromInclusive, int toExclusive, int maxDegreeOfParallelism);

        Task ForAsync(Func<int, Task> action, int fromInclusive, int toExclusive, int maxDegreeOfParallelism, Func<Task> iterationCompleted, CancellationToken? token = null);

        Task ForeachAsync<TSource>(IEnumerable<TSource> source, Func<TSource, Task> action, int maxDegreeOfParallelism);

        Task ForeachAsync<TSource>(IEnumerable<TSource> source, Func<TSource, Task> action, int maxDegreeOfParallelism, Func<Task> iterationCompleted);

        //Task ForEachAsync<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, Task<TResult>> body, int maxDegreeOfParallelism, Func<List<TResult>, Task> iterationCompleted);
    }
}
