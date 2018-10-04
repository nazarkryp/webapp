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
            var page = await _movieService.GetMoviesAsync(queryFilter);

            return Ok(page);
        }

        [HttpGet("{movieId:int}")]
        public async Task<IActionResult> GetMovieAsync(int movieId)
        {
            var movie = await _movieService.GetMovieAsync(movieId);

            return Ok(movie);
        }
    }
}