﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

        public async Task ForeachAsync<TSource>(IEnumerable<TSource> source, Func<TSource, Task<TSource>> action, int maxDegreeOfParallelism, Func<Task> iterationCompleted)
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

                    if (iterationCompleted != null)
                    {
                        await iterationCompleted();
                    }
                }
            }
        }

        public Task ForAsync(Func<int, Task> action, int fromInclusive, int toExclusive, int maxDegreeOfParallelism)
        {
            return ForAsync(action, fromInclusive, toExclusive, maxDegreeOfParallelism, null);
        }

        public async Task ForAsync(Func<int, Task> action, int fromInclusive, int toExclusive, int maxDegreeOfParallelism, Func<Task> iterationCompleted)
        {
            var tasks = new List<Task>(maxDegreeOfParallelism);

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

                        if (iterationCompleted != null)
                        {
                            await iterationCompleted();
                        }
                    }
                }
            }
        }

        //public Task ForEachAsync<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, Task<TResult>> body, int maxDegreeOfParallelism, Func<List<TResult>, Task> iterationCompleted)
        //{
        //    var partitions = Partitioner.Create(source)
        //        .GetPartitions(maxDegreeOfParallelism)
        //        .Select(partition => Task.Run(async delegate
        //        {
        //            using (partition)
        //            {
        //                while (partition.MoveNext())
        //                {
        //                    var result = await body(partition.Current);
        //                }
        //            }
        //        }));

        //    return Task.WhenAll(partitions);
        //}
    }
}