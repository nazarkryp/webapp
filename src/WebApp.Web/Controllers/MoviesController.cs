using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WebApp.Dto.Movies;
using WebApp.Services.Movies;

namespace WebApp.Web.Controllers
{
    //[Authorize]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMoviesAsync([FromQuery] MoviesQueryFilter queryFilter)
        {
            var watch = Stopwatch.StartNew();

            try
            {
                var page = await _movieService.GetMoviesAsync(queryFilter);

                watch.Stop();
                return Ok(page);
            }
            catch (Exception e)
            {
                watch.Stop();
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpGet("{movieId:int}")]
        public async Task<IActionResult> GetMovieAsync(int movieId)
        {
            return Ok(new { movieId });
        }
    }
}