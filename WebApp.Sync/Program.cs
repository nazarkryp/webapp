using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using WebApp.Domain.Entities;
using WebApp.Infrastructure.Handlers;
using WebApp.Mapping.AutoMapper.Mappers;
using WebApp.Repositories.EntityFramework.Context;
using WebApp.Repositories.EntityFramework.Repositories;
using WebApp.Repositories.Repositories;
using WebApp.Studios;
using WebApp.Studios.Brazzers;
using WebApp.Sync.Providers;

namespace WebApp.Sync
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await RunAsync();
        }

        private static async Task RunAsync()
        {
            IConcurrentActionHandler concurrentActionHandler = new ConcurrentActionHandler();
            IStudioClient studioClient = new BrazzersClient();
            IMovieRepository movieRepository = new MovieRepository(new WebAppDbContext(new DbContextOptions<WebAppDbContext>()), new MappingService());
            IStudioRepository studioRepository = new StudioRepository(new WebAppDbContext(new DbContextOptions<WebAppDbContext>()), new MappingService());
            ISyncDetailsRepository syncDetailsRepository = new SyncDetailsRepository(new WebAppDbContext(new DbContextOptions<WebAppDbContext>()), new MappingService());

            var movieSync = new MovieSync(concurrentActionHandler, studioClient, syncDetailsRepository, studioRepository, movieRepository);

            await movieSync.SyncAsync();

            //var collection = studioClient.GetPagesTasks(startPage);
            //var items = new List<SyncPage>();
            //await concurrentActionHandler.ForeachAsync(collection, async page =>
            //{
            //    var movies = await page.MoviesTask;
            //    var syncPage = new SyncPage
            //    {
            //        PageIndex = page.PageIndex,
            //        Movies = movies
            //    };

            //    items.Add(syncPage);

            //    return null;
            //}, 5, async () =>
            //{
            //    var pages = items.OrderBy(e => e.PageIndex);
            //    var studioMovies = pages.SelectMany(e => e.Movies);

            //    var movies = studioMovies.Select(e => new Movie
            //    {
            //        Title = e.Title,
            //        Attachments = e.Attachments.Select(attachment => new Attachment
            //        {
            //            Uri = attachment.Uri
            //        }),
            //        Date = e.Date,
            //        Duration = e.Duration,
            //        Description = !string.IsNullOrEmpty(e.Description) && e.Description.Length > 1000 ? e.Description.Substring(0, 1000) : e.Description,
            //        Uri = e.Uri,
            //        Studio = studio
            //    });

            //    await movieRepository.AddRangeAsync(movies);

            //    syncDetails.LastSyncDate = DateTime.UtcNow;
            //    syncDetails.LastSyncPage = pages.LastOrDefault().PageIndex;

            //    await syncDetailsRepository.UpdateAsync(syncDetails);
            //    Console.WriteLine($"Sync Details:\nStudio: {studio.Name}\nSyncPage: {syncDetails.LastSyncPage}\nSyncDate: {syncDetails.LastSyncDate}\n\n");

            //    items.Clear();
            //});

            //await concurrentActionHandler.ForAsync(async index =>
            //{
            //    var movies = await studioClient.GetPage(index);
            //    foreach (var movie in movies)
            //    {
            //        Console.WriteLine(movie.Title);
            //    }
            //}, syncDetails.LastSyncPage, 0, 5);

            //var studio = await studioRepository.FindAsync(studioClient.StudioName);

            //if (studio == null)
            //{
            //    studio = new Studio
            //    {
            //        Name = studioClient.StudioName
            //    };

            //    studio = await studioRepository.AddAsync(studio);
            //}

            //var syncDetails = await syncDetailsRepository.FindByStudioAsync(studio.StudioId);

            //int? startPage = null;
            //if (syncDetails?.LastSyncPage > 1)
            //{
            //    startPage = syncDetails.LastSyncPage - 1;
            //}


            //foreach (var page in studioClient.EnumeratePages(startPage))
            //{
            //    try
            //    {
            //        var movies = page.Item2.Select(e => new Movie
            //        {
            //            Title = e.Title,
            //            Attachments = e.Attachments.Select(attachment => new Attachment
            //            {
            //                Uri = attachment.Uri
            //            }),
            //            Date = e.Date,
            //            Duration = e.Duration,
            //            Description = !string.IsNullOrEmpty(e.Description) && e.Description.Length > 1000 ? e.Description.Substring(0, 1000) : e.Description,
            //            Uri = e.Uri,
            //            Studio = studio
            //        });

            //        await movieRepository.AddRangeAsync(movies);
            //        syncDetails = await UpdateSyncDetailsAsync(syncDetailsRepository, studio, syncDetails, page);

            //        Console.WriteLine($"Sync Details:\nStudio ${studio.Name}\nSyncPage: {syncDetails.LastSyncPage}\nSyncDate: {syncDetails.LastSyncDate}\n\n");
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e);
            //    }
            //}
        }

        private static async Task<SyncDetails> UpdateSyncDetailsAsync(ISyncDetailsRepository syncDetailsRepository, SyncDetails syncDetails, int studioId, int pageIndex)
        {
            if (syncDetails == null)
            {
                syncDetails = new SyncDetails
                {
                    LastSyncDate = DateTime.UtcNow,
                    LastSyncPage = pageIndex,
                    StudioId = studioId
                };

                await syncDetailsRepository.AddAsync(syncDetails);
            }
            else
            {
                syncDetails.LastSyncDate = DateTime.UtcNow;
                syncDetails.LastSyncPage = pageIndex;

                await syncDetailsRepository.UpdateAsync(syncDetails);
            }

            return syncDetails;
        }
    }

    public class SyncPage
    {
        public int PageIndex { get; set; }

        public IEnumerable<IMovie> Movies { get; set; }
    }
}
