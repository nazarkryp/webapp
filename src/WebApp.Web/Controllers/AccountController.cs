using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Dto.Account;
using WebApp.Infrastructure.Configuration;
using WebApp.Security;

namespace WebApp.Web.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationProvider _securityProvider;
        private readonly IConfigurationProvider _configurationProvider;

        public AccountController(IAuthenticationProvider securityProvider, IConfigurationProvider configurationProvider)
        {
            _securityProvider = securityProvider;
            _configurationProvider = configurationProvider;
        }

        [Authorize]
        public IActionResult GetAccount()
        {
            var claims = User.Claims.ToDictionary(e => e.Type, e => e.Value);

            var account = new Account
            {
                Email = User.Claims.FirstOrDefault(e => e.Type == ClaimTypes.Name)?.Value,
                Firstname = User.Claims.FirstOrDefault(e => e.Type == ClaimTypes.GivenName)?.Value,
                Lastname = User.Claims.FirstOrDefault(e => e.Type == ClaimTypes.Surname)?.Value,
                Picture = User.Claims.FirstOrDefault(e => e.Type == "picture")?.Value
            };

            return Ok(account);
        }

        [HttpGet("authorize")]
        public IActionResult SignIn()
        {
            var redirectUri = _securityProvider.RedirectUri;

            return Redirect(redirectUri);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code)
        {
            var accessToken = await _securityProvider.GetAccessToken(code);

            var baseAddress = _configurationProvider.Get("uiBaseAddress");
            return Redirect($"{baseAddress}#token={accessToken}");
        }
    }
}