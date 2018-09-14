﻿using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using WebApp.Dto.Common;
using WebApp.Services.Movies;

namespace WebApp.Web.Controllers
{
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
        public async Task<IActionResult> GetMoviesAsync([FromQuery] QueryFilter queryFilter)
        {
            var page = await _movieService.GetMoviesAsync(queryFilter);

            return Ok(page);
        }
    }
}