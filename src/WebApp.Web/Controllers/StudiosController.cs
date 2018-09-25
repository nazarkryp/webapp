using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WebApp.Services.Studios;

namespace WebApp.Web.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class StudiosController : ControllerBase
    {
        private readonly IStudioService _studioService;

        public StudiosController(IStudioService studioService)
        {
            _studioService = studioService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudiosAsync()
        {
            var studios = await _studioService.GetStudiosAsync();

            return Ok(studios);
        }

        [HttpGet("{studioId:int}")]
        public async Task<IActionResult> GetStudioByIdAsync(int studioId)
        {
            var studios = await _studioService.GetStudioByIdAsync(studioId);

            return Ok(studios);
        }
    }
}