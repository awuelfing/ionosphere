using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace DXws.Controllers.Web
{
    [Route("/account")]
    public class WebAccountController : Controller
    {
        [Route("Index")]
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("not really");
        }
        [HttpGet]
        [Route("Login")]
        public async Task<IActionResult> Login()
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, "User2"));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Hello"));
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            AuthenticationProperties authenticationProperties = new AuthenticationProperties()
            {
                IsPersistent = true,
                AllowRefresh = true,
                ExpiresUtc = DateTime.UtcNow.AddDays(100)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authenticationProperties);

            return Ok();
        }
        [HttpGet]
        [Route("Secure")]
        [Authorize(Roles ="testing")]
        public IActionResult Secure()
        {
            return Ok("Yes good");
        }
    }
}
