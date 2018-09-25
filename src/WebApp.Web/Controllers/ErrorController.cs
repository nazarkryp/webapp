using Microsoft.AspNetCore.Mvc;

namespace WebApp.Web.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("{code}")]
        [HttpGet, HttpPost, HttpPut, HttpDelete, HttpHead, HttpOptions]
        public IActionResult Error()
        {
            return Ok();
        }
    }
}