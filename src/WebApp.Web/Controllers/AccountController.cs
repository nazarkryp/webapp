using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using WebApp.Security;
using WebApp.Security.Configuration;

namespace WebApp.Web.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationProvider _securityProvider;

        public AccountController(IAuthenticationProvider securityProvider)
        {
            _securityProvider = securityProvider;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Redirect(_securityProvider.RedirectUri);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback()
        {
            var code = this.Request.Query["code"];

            var token = await _securityProvider.GetAccessToken(code);

            return Redirect($"http://localhost:4200#id_token={token.IdToken}");
            //return Ok(new
            //{
            //    name = User.Identity.Name,
            //    isAuthenticated = User.Identity.IsAuthenticated,
            //    token = token
            //});
        }
    }
}