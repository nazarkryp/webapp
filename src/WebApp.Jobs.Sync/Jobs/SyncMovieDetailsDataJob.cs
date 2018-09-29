using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using WebApp.Domain.Entities;
using WebApp.Infrastructure.Handlers;
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
        private readonly IConcurrentActionHandler _concurrentActionHandler;

        public SyncMovieDetailsDataJob(IStudioRepository studioRepository, IMovieRepository movieRepository, IMapper mapper, IConcurrentActionHandler concurrentActionHandler)
        {
            _studioRepository = studioRepository;
            _movieRepository = movieRepository;
            _mapper = mapper;
            _concurrentActionHandler = concurrentActionHandler;
        }

        public async Task SyncMovieDetailsAsync(IStudioClient studioClient)
        {
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
            int counter = 0;

            await _concurrentActionHandler.ForeachAsync(movies,
                async movie =>
                {
                    try
                    {
                        Console.WriteLine($"{counter}. {movie.Uri}");
                        var details = await studioClient.GetMovieDetailsAsync(movie.Uri);
                        Interlocked.Increment(ref counter);

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

                        buffer.Add(movie);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR:");
                        Console.WriteLine(e.Message);
                    }
                }, 3, async () =>
                {
                    var moviesToUpdate = buffer.ToList();
                    buffer.Clear();

                    if (moviesToUpdate.Any())
                    {
                        await _movieRepository.UpdateAsync(moviesToUpdate);

                        Console.WriteLine("Success");
                    }
                });
        }
    }
}
