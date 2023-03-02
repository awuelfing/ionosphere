using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using DXws.Models.Web;
using DxLib.DbCaching;
using DXws.Admin;
using Microsoft.IdentityModel.Logging;

namespace DXws.Controllers.Web
{
    [Route("/account")]
    public class WebAccountController : Controller
    {
        private readonly ILogger<WebAccountController> _logger;
        private readonly DbUser _dbUser;
        private readonly IConfiguration _configuration;
        public WebAccountController(ILogger<WebAccountController> logger,DbUser dbUser,IConfiguration configuration)
        {
            _logger = logger;
            _dbUser = dbUser;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("Login")]
        public IActionResult LoginPage()
        {
            return View("Login");
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = await _dbUser.GetUser(loginViewModel.Username??"");
            _logger.Log(LogLevel.Trace, "Login attempt for {user}", loginViewModel.Username);
            if (user != null && user.Password == loginViewModel.Password)
            {
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    AdminHelp.CreateClaimsUserRoles(user.Username,user.Roles),
                    AdminHelp.GetAuthenticationProperties(100));
                _logger.Log(LogLevel.Information, "Login attempt for {user} succeeded", user.Username);
                return RedirectToAction("Index", "WebHome");
            }
            _logger.Log(LogLevel.Warning, "Login attempt for {user} failed", loginViewModel.Username);
            return RedirectToAction("Index", "WebHome");
        }
        [HttpGet]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            var user = AdminHelp.GetUser(HttpContext.User.Claims);
            await HttpContext.SignOutAsync();
            _logger.Log(LogLevel.Information, "Logout attempt for {user} succeeded", user);
            return RedirectToAction("Index", "WebHome");
        }
        [HttpGet]
        [Route("GenerateJWT")]
        [Authorize]
        public async Task<IActionResult> GenerateJWT()
        {
            var username = AdminHelp.GetUser(HttpContext.User.Claims);
            var user = await _dbUser.GetUser(username);
            var key = _configuration.GetValue<string>("JwtKey")??"";
            ViewData["JWT"] = AdminHelp.GenerateJWT(user,key);
            return View();
        }
    }
}
