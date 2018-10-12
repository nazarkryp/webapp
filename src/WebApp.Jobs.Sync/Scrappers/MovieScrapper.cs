using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Handlers;
using WebApp.Jobs.Sync.Configuration;
using WebApp.Mapping;
using WebApp.Repositories.Repositories;
using WebApp.Studios;

namespace WebApp.Jobs.Sync.Scrappers
{
    internal class MovieScrapper
    {
        private readonly IConcurrentActionHandler _concurrentActionHandler;
        private readonly ISyncConfiguration _syncConfiguration;
        private readonly ISyncDetailsRepository _syncDetailsRepository;
        private readonly IStudioRepository _studioRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;

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
                var syncInformation = new SyncQueueItem
                {
                    SyncDetails = details,
                    Completed = false
                };

                syncQueue.Add(syncInformation);
            }
        }

        private async Task<SyncDetails> GetSyncDetailsAsync(IStudioClient studioClient)
        {
            var studio = await GetStudioAsync(studioClient);
            var syncDetails = await _syncDetailsRepository.FindByStudioAsync(studio.StudioId);

            return syncDetails;
        }

        #region Private Methods

        private async Task StartGettingMoviesAsync(IStudioClient studioClient, int start, int end, Func<object, Task> pagesScraped)
        {
            await _concurrentActionHandler.ForAsync(
                async pageIndex => await studioClient.GetMoviesAsync(pageIndex),
                start,
                end,
                _syncConfiguration.MaxDegreeOfParallelism, pagesScraped);
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

        #endregion
    }
}
