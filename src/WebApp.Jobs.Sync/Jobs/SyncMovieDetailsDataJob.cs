using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using WebApp.Domain.Entities;
using WebApp.Infrastructure.Handlers;
using WebApp.Jobs.Sync.Infrastructure.Communication;
using WebApp.Mapping;
using WebApp.Repositories.Repositories;
using WebApp.Studios;

namespace WebApp.Jobs.Sync.Jobs
{
    public class SyncMovieDetailsDataJob : IDetailsJob
    {
        private readonly IStudioRepository _studioRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;
        private readonly IScrapperClient _scrapperClient;
        private static IConcurrentActionHandler _concurrentActionHandler;

        private static int total = 0;
        private static int counter = 0;

        public SyncMovieDetailsDataJob(
            IStudioRepository studioRepository,
            IMovieRepository movieRepository,
            IMapper mapper,
            IConcurrentActionHandler concurrentActionHandler,
            IScrapperClient scrapperClient)
        {
            _studioRepository = studioRepository;
            _movieRepository = movieRepository;
            _mapper = mapper;
            _concurrentActionHandler = concurrentActionHandler;
            _scrapperClient = scrapperClient;
        }

        public async Task SyncMovieDetailsAsync(IStudioClient studioClient)
        {
            var stopwatch = Stopwatch.StartNew();

            Console.Write($"Retrieving {studioClient.StudioName} details. ");
            var studio = await _studioRepository.FindAsync(studioClient.StudioName);
            Console.WriteLine($"Studio ID: {studio.StudioId}");
            Console.WriteLine("Retrieving movies...");
            var movies = await _movieRepository.FindAllStudioMoviesAsync(studio.StudioId);
            Console.WriteLine("Getting movie details\n");

            //var sex = await _movieRepository.FindMovieAsync(17889);

            //var movies = new List<Movie>
            //{
            //    sex
            //};

            var buffer = new ConcurrentBag<Movie>();
            total += movies.Count();

            var errorsCount = 0;
            await _concurrentActionHandler.ForeachAsync(movies, async movie =>
            {
                try
                {
                    Interlocked.Increment(ref counter);
                    
                    var scrapedMovie = await GetMovieAsync(studioClient, movie);

                    buffer.Add(scrapedMovie);

                    if (errorsCount > 0)
                    {
                        Interlocked.Decrement(ref errorsCount);
                    }
                }
                catch (Exception e)
                {
                    Interlocked.Increment(ref errorsCount);

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("SCRAPPING ERROR:");
                    Console.WriteLine(e.Message);
                    Console.ForegroundColor = ConsoleColor.White;

                    if (errorsCount == 5)
                    {
                        throw;
                    }
                }
                finally
                {
                    stopwatch.Stop();
                }
            }, 4, () => IterationCompleted(buffer));
        }

        private async Task<Movie> GetMovieAsync(IStudioClient studioClient, Movie movie)
        {
            var content = await _scrapperClient.GetAsync(movie.Uri);
            var details = await studioClient.ParseDetailsAsync(content);

            movie.Description = details.Description;
            movie.Categories = _mapper.Map<IEnumerable<Category>>(details.Categories);
            movie.Duration = details.Duration;

            if (details.Attachments != null)
            {
                movie.Attachments = _mapper.Map<IEnumerable<Attachment>>(details.Attachments);
            }

            if (details.Models != null)
            {
                movie.Models = _mapper.Map<IEnumerable<Model>>(details.Models);
            }

            return movie;
        }

        private async Task IterationCompleted(ConcurrentBag<Movie> buffer)
        {
            try
            {
                var moviesToUpdate = buffer.ToList();
                buffer.Clear();

                var withoutDescription = moviesToUpdate
                    .Where(e => string.IsNullOrEmpty(e.Description))
                    .Select(e => e.Description).ToArray();

                if (withoutDescription.Any())
                {
                    var stream = File.OpenWrite(@"C:\Users\nazarkryp\Desktop\Logs\without-description.txt");
                    using (var writer = new StreamWriter(stream))
                    {
                        foreach (var item in withoutDescription)
                        {
                            writer.WriteLine(item);
                        }
                    }

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"\n\nMovied without description found: {withoutDescription.Length}\n\n");
                    Console.ForegroundColor = ConsoleColor.White;
                }

                if (moviesToUpdate.Any())
                {
                    Console.WriteLine("Updating database");
                    await _movieRepository.UpdateAsync(moviesToUpdate);
                    Console.WriteLine($"Updated: {moviesToUpdate.Count}\n");
                }

                var progress = ((double)counter * 100) / total;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Progress: {progress}%; Items left: {total - counter}\n");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception e)
            {
                Console.WriteLine($"DATABASE ERROR: {e.Message}");
                throw;
            }
        }
    }
}
