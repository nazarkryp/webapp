using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using WebApp.Dto.Categories;
using WebApp.Dto.Common;
using WebApp.Services.Categories;

namespace WebApp.Web.Controllers
{
    //[Authorize]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoriesAsync()
        {
            var categories = await _categoryService.GetCategoriesAsync();

            var response = new DataResponse<Category>(categories);

            return Ok(response);
        }
    }
}