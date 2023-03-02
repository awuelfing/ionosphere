using Microsoft.AspNetCore.Mvc;

namespace DXws.Controllers.Web
{
    [Route("/web")]
    public class WebHomeController : Controller
    {
        [HttpGet]
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
