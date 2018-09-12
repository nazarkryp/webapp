using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApp.Infrastructure.Handlers
{
    public class ConcurrentActionHandler : IConcurrentActionHandler
    {
        public async Task ForeachAsync<TSource>(IEnumerable<TSource> source, Func<TSource, Task> action, int maxDegreeOfParallelism)
        {
            if (maxDegreeOfParallelism < 1)
            {
                throw new ArgumentException($"Invalid {nameof(maxDegreeOfParallelism)}", nameof(maxDegreeOfParallelism));
            }

            var tasks = new List<Task>();

            foreach (var item in source)
            {
                tasks.Add(action(item));

                if (tasks.Count == maxDegreeOfParallelism)
                {
                    await Task.WhenAll(tasks);

                    tasks.Clear();
                }
            }
        }

        public Task ForeachAsync<TSource>(IEnumerable<TSource> source, Func<TSource, Task<TSource>> action, int maxDegreeOfParallelism)
        {
            return ForeachAsync(source, action, maxDegreeOfParallelism, null);
        }

        public async Task ForeachAsync<TSource>(IEnumerable<TSource> source, Func<TSource, Task<TSource>> action, int maxDegreeOfParallelism, Action degreeCompleted)
        {
            if (maxDegreeOfParallelism < 1)
            {
                throw new ArgumentException($"Invalid {nameof(maxDegreeOfParallelism)}", nameof(maxDegreeOfParallelism));
            }

            var tasks = new List<Task>();

            foreach (var item in source)
            {
                tasks.Add(action(item));

                if (tasks.Count == maxDegreeOfParallelism)
                {
                    await Task.WhenAll(tasks);

                    tasks.Clear();

                    if (degreeCompleted != null)
                    {
                        degreeCompleted();
                    }
                }
            }
        }

        public Task ForAsync(Func<int, Task> action, int fromInclusive, int toExclusive, int maxDegreeOfParallelism)
        {
            return ForAsync(action, fromInclusive, toExclusive, maxDegreeOfParallelism, null);
        }

        public async Task ForAsync(Func<int, Task> action, int fromInclusive, int toExclusive, int maxDegreeOfParallelism, Action iterationCompleted)
        {
            var tasks = new List<Task>();

            if (fromInclusive < toExclusive)
            {
                for (; fromInclusive < toExclusive; fromInclusive++)
                {
                    tasks.Add(action(fromInclusive));

                    if (tasks.Count == maxDegreeOfParallelism)
                    {
                        await Task.WhenAll(tasks);
                        tasks.Clear();
                    }
                }
            }
            else if (fromInclusive > toExclusive)
            {
                for (; fromInclusive > toExclusive; fromInclusive--)
                {
                    tasks.Add(action(fromInclusive));

                    if (tasks.Count == maxDegreeOfParallelism)
                    {
                        await Task.WhenAll(tasks);
                        tasks.Clear();
                        iterationCompleted();
                    }
                }
            }
        }
    }
}
