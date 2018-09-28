using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
            var studio = await _studioRepository.FindAsync(studioClient.StudioName);
            var movies = await _movieRepository.FindAllStudioMoviesAsync(studio.StudioId);

            var buffer = new ConcurrentBag<Movie>();

            await _concurrentActionHandler.ForeachAsync(movies,
                async movie =>
                {
                    try
                    {
                        var details = await studioClient.GetMovieDetailsAsync(movie.Uri);

                        movie.Description = details.Description;
                        movie.Categories = _mapper.Map<IEnumerable<Category>>(details.Categories);
                        movie.Duration = details.Duration;

                        buffer.Add(movie);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }, 5, async () =>
                {
                    var moviesToUpdate = buffer.ToList();
                    buffer.Clear();

                    await _movieRepository.UpdateAsync(moviesToUpdate);
                });
        }
    }
}
