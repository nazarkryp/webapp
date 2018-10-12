using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WebApp.Infrastructure.Handlers
{
    public interface IConcurrentActionHandler
    {
        Task ForAsync<TResult>(Func<int, Task<TResult>> action, int fromInclusive, int toExclusive, int maxDegreeOfParallelism);

        Task ForAsync<TResult>(Func<int, Task<TResult>> action, int fromInclusive, int toExclusive, int maxDegreeOfParallelism, Func<object, Task> iterationCompleted, CancellationToken? token = null);

        Task ForeachAsync<TSource>(IEnumerable<TSource> source, Func<TSource, Task> action, int maxDegreeOfParallelism);

        Task ForeachAsync<TSource>(IEnumerable<TSource> source, Func<TSource, Task> action, int maxDegreeOfParallelism, Func<Task> iterationCompleted);

        //Task ForEachAsync<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, Task<TResult>> body, int maxDegreeOfParallelism, Func<List<TResult>, Task> iterationCompleted);
    }
}
