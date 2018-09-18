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

namespace WebApp.Jobs.Sync.Jobs
{
    internal class SyncMoviesDataJob : IJob
    {
        private readonly IConcurrentActionHandler _concurrentActionHandler;
        private readonly ISyncConfiguration _syncConfiguration;
        private readonly ISyncDetailsRepository _syncDetailsRepository;
        private readonly IStudioRepository _studioRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;

        public SyncMoviesDataJob(
            ISyncDetailsRepository syncDetailsRepository,
            IStudioRepository studioRepository,
            IMovieRepository movieRepository,
            IConcurrentActionHandler concurrentActionHandler,
            ISyncConfiguration syncConfiguration,
            IMapper mapper)
        {
            _concurrentActionHandler = concurrentActionHandler;
            _syncConfiguration = syncConfiguration;
            _mapper = mapper;
            _syncDetailsRepository = syncDetailsRepository;
            _studioRepository = studioRepository;
            _movieRepository = movieRepository;
        }

        public async Task SyncAsync(IStudioClient studioClient)
        {
            var studio = await GetStudioAsync(studioClient);
            var syncDetails = await _syncDetailsRepository.FindByStudioAsync(studio.StudioId);

            if (syncDetails == null)
            {
                var startFrom = await studioClient.GetPagesCountAsync();
                await SyncMoviesAsync(studioClient, startFrom, studio.StudioId, null);
            }
            else if (syncDetails.LastSyncPage - 1 >= 1)
            {
                var startFrom = syncDetails.LastSyncPage - 1;
                await SyncMoviesAsync(studioClient, startFrom, studio.StudioId, syncDetails);
            }
            else
            {
                await AppendMoviesAsync(studioClient, studio.StudioId);
            }
        }

        private async Task SyncMoviesAsync(IStudioClient studioClient, int startFrom, int studioId, SyncDetails syncDetails)
        {
            var buffer = new ConcurrentDictionary<int, IEnumerable<IMovie>>();

            await _concurrentActionHandler.ForAsync(async pageIndex =>
            {
                var movies = await studioClient.GetMoviesAsync(pageIndex);
                buffer.TryAdd(pageIndex, movies);
            }, startFrom, 0, _syncConfiguration.MaxDegreeOfParallelism, async () => { syncDetails = await SaveAsync(buffer, syncDetails, studioId); });
        }

        private async Task<SyncDetails> SaveAsync(ConcurrentDictionary<int, IEnumerable<IMovie>> buffer, SyncDetails syncDetails, int studioId)
        {
            var pages = buffer.OrderByDescending(e => e.Key).ToList();

            if (pages.Any())
            {
                var studioMovies = pages.SelectMany(e => e.Value.Reverse());
                var moviesToSave = MapMovies(studioMovies, studioId);

                var saved = await _movieRepository.AddRangeAsync(moviesToSave);

                var first = pages.LastOrDefault();
                syncDetails = await UpdateSyncDetailsAsync(syncDetails, studioId, first.Key);

                Console.WriteLine($"Synched ({buffer.Count} pages):\n\nLast sync page: {syncDetails.LastSyncPage}\n\nMovies saved: {saved.Count()}\n\n");

                buffer.Clear();
            }

            return syncDetails;
        }

        private async Task AppendMoviesAsync(IStudioClient studioClient, int studioId)
        {
            var existingMovies = await _movieRepository.LatestAsync(studioId);

            var buffer = new ConcurrentDictionary<int, IEnumerable<IMovie>>();
            var cts = new CancellationTokenSource();

            await _concurrentActionHandler.ForAsync(async pageIndex =>
            {
                var movies = await studioClient.GetMoviesAsync(pageIndex);
                buffer.TryAdd(pageIndex, movies);
            }, 1, 100, _syncConfiguration.MaxDegreeOfParallelism, async () =>
            {
                var pages = buffer.OrderBy(e => e.Key).ToList();
                buffer.Clear();

                if (pages.Any())
                {
                    var studioMovies = pages.SelectMany(e => e.Value);

                    studioMovies = studioMovies.Where(studioMovie => existingMovies.All(existingMovie => !string.Equals(existingMovie.Uri, studioMovie.Uri, StringComparison.CurrentCultureIgnoreCase) && existingMovie.Date <= studioMovie.Date));

                    if (!studioMovies.Any())
                    {
                        cts.Cancel();
                        Console.WriteLine($"Append Completed;\n\n");
                    }
                    else
                    {
                        var moviesToSave = MapMovies(studioMovies, studioId);
                        var saved = await _movieRepository.AddRangeAsync(moviesToSave);

                        Console.WriteLine($"Append success ({saved.Count()} pages);\n\n");
                    }
                }
            }, cts.Token);
        }

        private async Task<Studio> GetStudioAsync(IStudioClient studioClient)
        {
            var studio = await _studioRepository.FindAsync(studioClient.StudioName);

            if (studio == null)
            {
                studio = new Studio
                {
                    Name = studioClient.StudioName
                };

                studio = await _studioRepository.AddAsync(studio);
            }

            return studio;
        }

        private async Task<SyncDetails> UpdateSyncDetailsAsync(SyncDetails syncDetails, int studioId, int pageIndex)
        {
            if (syncDetails == null)
            {
                syncDetails = new SyncDetails
                {
                    Studio = new Studio
                    {
                        StudioId = studioId
                    }
                };
            }

            syncDetails.LastSyncDate = DateTime.UtcNow;
            syncDetails.LastSyncPage = pageIndex;

            if (syncDetails.SyncDetailsId == 0)
            {
                var saved = await _syncDetailsRepository.AddAsync(syncDetails);
                syncDetails.SyncDetailsId = saved.SyncDetailsId;
            }
            else
            {
                await _syncDetailsRepository.UpdateAsync(syncDetails);
            }

            return syncDetails;
        }

        private IEnumerable<Movie> MapMovies(IEnumerable<IMovie> studioMovies, int studioId)
        {
            var movies = _mapper.Map<IEnumerable<Movie>>(studioMovies).ToList();

            foreach (var movie in movies)
            {
                movie.Studio = new Studio
                {
                    StudioId = studioId
                };
            }

            return movies;
        }
    }
}
