using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WebApp.Services.Models;

namespace WebApp.Web.Controllers
{
    //// [Authorize]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ModelsController : ControllerBase
    {
        private readonly IModelService _modelService;

        public ModelsController(IModelService modelService)
        {
            _modelService = modelService;
        }

        [HttpGet, Route("{modelId:int}")]
        public async Task<IActionResult> GetModelAsync(int modelId)
        {
            var result = await _modelService.GetModelByIdAsync(modelId);

            return Ok(result);
        }
    }
}