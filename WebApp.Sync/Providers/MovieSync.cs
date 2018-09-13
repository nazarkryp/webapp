using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Handlers;
using WebApp.Repositories.Repositories;
using WebApp.Studios;
using WebApp.Sync.Models;

namespace WebApp.Sync.Providers
{
    public class MovieSync
    {
        private readonly IConcurrentActionHandler _concurrentActionHandler;
        private readonly IStudioClient _studioClient;
        private readonly ISyncDetailsRepository _syncDetailsRepository;
        private IStudioRepository _studioRepository;
        private IMovieRepository _movieRepository;

        public MovieSync(
            IConcurrentActionHandler concurrentActionHandler,
            IStudioClient studioClient,
            ISyncDetailsRepository syncDetailsRepository,
            IStudioRepository studioRepository,
            IMovieRepository movieRepository)
        {
            _concurrentActionHandler = concurrentActionHandler;
            _studioClient = studioClient;
            _syncDetailsRepository = syncDetailsRepository;
            _studioRepository = studioRepository;
            _movieRepository = movieRepository;
        }

        public async Task SyncAsync()
        {
            var to = 0;
            var maxDegreeOfParallelism = 5;
            var studio = await GetStudioAsync();
            var syncDetails = await GetSyncDetailsAsync(studio);
            var buffer = new ConcurrentBag<SyncObject<IMovie>>();

            await _concurrentActionHandler.ForAsync(async pageIndex =>
            {
                var movies = await _studioClient.GetPageAsync(pageIndex);

                var syncObject = new SyncObject<IMovie>
                {
                    PageIndex = pageIndex,
                    Items = movies
                };

                buffer.Add(syncObject);
            }, syncDetails.LastSyncPage, to, maxDegreeOfParallelism, async () =>
            {
                var pages = buffer.OrderBy(e => e.PageIndex).ToList();

                if (pages.Any())
                {
                    var first = pages.FirstOrDefault();
                    syncDetails.LastSyncDate = DateTime.UtcNow;
                    syncDetails.LastSyncPage = first.PageIndex;

                    var studioMovies = pages.SelectMany(e => e.Items);

                    var movies = MapMovies(studioMovies, studio);
                    await _movieRepository.AddRangeAsync(movies);

                    await _syncDetailsRepository.UpdateAsync(syncDetails);

                    Console.WriteLine($"Sync Details:\nStudio: {studio.Name}\nSyncPage: {syncDetails.LastSyncPage}\nSyncDate: {syncDetails.LastSyncDate}\n\n");

                    buffer.Clear();
                }
            });
        }

        private async Task<SyncDetails> GetSyncDetailsAsync(Studio studio)
        {
            var syncDetails = await _syncDetailsRepository.FindByStudioAsync(studio.StudioId);

            if (syncDetails == null)
            {
                var totalPages = await _studioClient.GetPagesCountAsync();

                syncDetails = new SyncDetails
                {
                    StudioId = studio.StudioId,
                    LastSyncDate = DateTime.Now,
                    LastSyncPage = totalPages
                };

                syncDetails = await _syncDetailsRepository.AddAsync(syncDetails);
            }

            return syncDetails;
        }

        private async Task<Studio> GetStudioAsync()
        {
            var studio = await _studioRepository.FindAsync(_studioClient.StudioName);

            if (studio == null)
            {
                studio = new Studio
                {
                    Name = _studioClient.StudioName
                };

                studio = await _studioRepository.AddAsync(studio);
            }

            return studio;
        }

        private static IEnumerable<Movie> MapMovies(IEnumerable<IMovie> studioMovies, Studio studio)
        {
            return studioMovies.Select(e => new Movie
            {
                Title = e.Title,
                Attachments = e.Attachments.Select(attachment => new Attachment
                {
                    Uri = attachment.Uri
                }),
                Date = e.Date,
                Duration = e.Duration,
                Description = !string.IsNullOrEmpty(e.Description) ? e.Description : null,
                Uri = e.Uri,
                Studio = studio
            });
        }
    }
}
