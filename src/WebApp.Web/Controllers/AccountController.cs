using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Web.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("/")
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public IActionResult Callback()
        {
            return Ok(new
            {
                name = User.Identity.Name,
                isAuthenticated = User.Identity.IsAuthenticated
            });
        }
    }
}