using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using WebApp.Domain.Entities;
using WebApp.Infrastructure.Handlers;
using WebApp.Jobs.Sync.Configuration;
using WebApp.Mapping;
using WebApp.Repositories.Repositories;
using WebApp.Studios;

namespace WebApp.Jobs.Sync.Scrappers
{
    internal class MovieScrapper : IScrapper
    {
        private readonly IConcurrentActionHandler _concurrentActionHandler;
        private readonly ISyncConfiguration _syncConfiguration;
        private readonly ISyncDetailsRepository _syncDetailsRepository;
        private readonly IStudioRepository _studioRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;

        private int counter = 0;
        private readonly ConcurrentBag<Movie> _buffer = new ConcurrentBag<Movie>();
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public MovieScrapper(
            IConcurrentActionHandler concurrentActionHandler,
            ISyncConfiguration syncConfiguration,
            ISyncDetailsRepository syncDetailsRepository,
            IStudioRepository studioRepository,
            IMovieRepository movieRepository,
            IMapper mapper)
        {
            _concurrentActionHandler = concurrentActionHandler;
            _syncConfiguration = syncConfiguration;
            _syncDetailsRepository = syncDetailsRepository;
            _studioRepository = studioRepository;
            _movieRepository = movieRepository;
            _mapper = mapper;
        }

        public async Task ScrapMoviesAsync(params IStudioClient[] studioClients)
        {
            IList<SyncQueueItem> syncQueue = new List<SyncQueueItem>();

            foreach (var studioClient in studioClients)
            {
                var details = await GetSyncDetailsAsync(studioClient);

                var queueItem = new SyncQueueItem
                {
                    SyncDetails = details,
                    StudioClient = studioClient
                };

                syncQueue.Add(queueItem);
            }

            await ProcessScrappingAsync(syncQueue);
        }

        private async Task ProcessScrappingAsync(IEnumerable<SyncQueueItem> syncQueue)
        {
            var tasks = new List<Task>(2);

            foreach (var item in syncQueue)
            {
                if (item.SyncDetails == null)
                {
                    var startFrom = await item.StudioClient.GetPagesCountAsync();
                    var task = StartGettingMoviesAsync(item.StudioClient, startFrom, 0, SaveMoviesAsync, null, item.SyncDetails.Studio);
                    tasks.Add(task);
                }
                else if (item.SyncDetails.LastSyncPage - 1 >= 1)
                {
                    var startFrom = item.SyncDetails.LastSyncPage - 1;
                    var task = StartGettingMoviesAsync(item.StudioClient, startFrom, 0, SaveMoviesAsync, null, item.SyncDetails.Studio);
                    tasks.Add(task);
                }
                else
                {
                    var lastestMovies = await _movieRepository.FindLatestAsync(item.SyncDetails.Studio.StudioId);
                    var task = StartGettingMoviesAsync(item.StudioClient, 1, 2, SaveMoviesAsync, lastestMovies, item.SyncDetails.Studio);
                    tasks.Add(task);
                }
            }

            await Task.WhenAll(tasks);
        }

        private async Task SaveMoviesAsync(IEnumerable<Movie> items)
        {
            Interlocked.Increment(ref counter);

            foreach (var studioMovie in items)
            {
                _buffer.Add(studioMovie);
            }

            if (counter == 2)
            {
                var itemsToSave = _buffer.OrderByDescending(e => e.Date).ToList();
                _buffer.Clear();
                Interlocked.Exchange(ref counter, 0);

                Console.WriteLine($"Total {itemsToSave.Count} items to save");
                await _movieRepository.AddRangeAsync(itemsToSave);
                Console.WriteLine("Saved");
            }

            await Task.Delay(10);
        }

        #region Private Methods

        private async Task<SyncDetails> GetSyncDetailsAsync(IStudioClient studioClient)
        {
            var studio = await GetStudioAsync(studioClient);
            var syncDetails = await _syncDetailsRepository.FindByStudioAsync(studio.StudioId);

            return syncDetails;
        }

        private async Task<Domain.Entities.Studio> GetStudioAsync(IStudioClient studioClient)
        {
            var studio = await _studioRepository.FindAsync(studioClient.StudioName);

            if (studio == null)
            {
                studio = new Domain.Entities.Studio
                {
                    Name = studioClient.StudioName
                };

                studio = await _studioRepository.AddAsync(studio);
            }

            return studio;
        }

        private async Task StartGettingMoviesAsync(IStudioClient studioClient, int start, int end, Func<IEnumerable<Movie>, Task> pagesScraped, IEnumerable<Movie> latest, WebApp.Domain.Entities.Studio studio)
        {
            var completed = 0;
            var cts = new CancellationTokenSource();

            await _concurrentActionHandler.ForAsync(
                async pageIndex =>
                {
                    Console.WriteLine($"Getting {studioClient.StudioName} - {pageIndex}");
                    var movies = await studioClient.GetMoviesAsync(pageIndex);
                    var studioMovies = movies as StudioMovie[] ?? movies.ToArray();

                    if (latest != null && studioMovies.Any(e => latest.Any(l => l.Uri == e.Uri || l.Date > e.Date)))
                    {
                        Interlocked.Increment(ref completed);
                        Console.WriteLine($"Need to cancel: {studioClient.StudioName}");
                        var enumerable = latest as Movie[] ?? latest.ToArray();
                        var date = enumerable.OrderByDescending(e => e.Date).LastOrDefault()?.Date;
                        var result =  studioMovies.Where(e => enumerable.All(l => !UrlsEqual(e.Uri, l.Uri)) && e.Date >= date);

                        return result;
                    }

                    return studioMovies;
                }, start, end, _syncConfiguration.MaxDegreeOfParallelism, async (result) =>
                {
                    await semaphore.WaitAsync();

                    if (completed > 0)
                    {
                        cts.Cancel();
                    }

                    var movies = result.SelectMany(e => e);

                    var moviesToSave = _mapper.Map<IEnumerable<Movie>>(movies);

                    foreach (var movie in moviesToSave)
                    {
                        movie.Studio = studio;
                    }

                    await pagesScraped(moviesToSave);

                    semaphore.Release();
                }, cts.Token);
        }

        private static bool UrlsEqual(string url1, string url2)
        {
            var uri1 = new Uri(url1);

            if (!string.IsNullOrEmpty(uri1.Query))
            {
                url1 = url1.Remove(url1.IndexOf(uri1.Query, StringComparison.CurrentCultureIgnoreCase), uri1.Query.Length);
            }

            var uri2 = new Uri(url2);

            if (!string.IsNullOrEmpty(uri2.Query))
            {
                url2 = url2.Remove(url2.IndexOf(uri2.Query, StringComparison.CurrentCultureIgnoreCase), uri2.Query.Length);
            }

            return url1 == url2;
        }

        #endregion
    }
}
