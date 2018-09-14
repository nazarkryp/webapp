using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
            int startFrom;
            int to = 0;

            var studio = await GetStudioAsync(studioClient);
            var syncDetails = await _syncDetailsRepository.FindByStudioAsync(studio.StudioId);

            if (syncDetails == null)
            {
                startFrom = await studioClient.GetPagesCountAsync();
            }
            else if (syncDetails.LastSyncPage == 1)
            {
                startFrom = 1;
                to = 100;
            }
            else
            {
                startFrom = syncDetails.LastSyncPage - 1;
            }

            var buffer = new ConcurrentDictionary<int, IEnumerable<IMovie>>();

            await _concurrentActionHandler.ForAsync(async pageIndex =>
            {
                var movies = await studioClient.GetMoviesAsync(pageIndex);
                buffer.TryAdd(pageIndex, movies);
            }, startFrom, to, _syncConfiguration.MaxDegreeOfParallelism, () => SaveMoviesAsync(buffer, syncDetails, studio.StudioId));
        }

        private async Task SaveMoviesAsync(ConcurrentDictionary<int, IEnumerable<IMovie>> buffer, SyncDetails syncDetails, int studioId)
        {
            var pages = buffer.OrderBy(e => e.Key).ToList();

            if (pages.Any())
            {
                var first = pages.FirstOrDefault();

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
                syncDetails.LastSyncPage = first.Key;

                var studioMovies = pages.SelectMany(e => e.Value);

                var movies = MapMovies(studioMovies, studioId);

                await _movieRepository.AddRangeAsync(movies);

                if (syncDetails.SyncDetailsId == 0)
                {
                    await _syncDetailsRepository.AddAsync(syncDetails);
                }
                else
                {
                    await _syncDetailsRepository.UpdateAsync(syncDetails);
                }


                Console.WriteLine($"Sync Success ({buffer.Count} pages):\n\nSyncPage: {syncDetails.LastSyncPage}\n\n");

                buffer.Clear();
            }
        }

        private async Task<SyncDetails> CreateSyncDetailsAsync(IStudioClient studioClient, Studio studio)
        {
            var totalPages = await studioClient.GetPagesCountAsync();

            var syncDetails = new SyncDetails
            {
                Studio = studio,
                LastSyncDate = DateTime.Now,
                LastSyncPage = totalPages
            };

            return await _syncDetailsRepository.AddAsync(syncDetails);
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

        private IEnumerable<Movie> MapMovies(IEnumerable<IMovie> studioMovies, int studioId)
        {
            var movies = _mapper.Map<IList<Movie>>(studioMovies);

            foreach (var movie in movies)
            {
                movie.Studio = new Studio
                {
                    StudioId = studioId
                };
            }

            return movies;
        }

        private async Task AppendAsync()
        {

        }
    }
}
