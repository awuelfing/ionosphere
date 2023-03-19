using Microsoft.AspNetCore.Mvc;
using DXLib.Maidenhead;

namespace DXws.Controllers
{
    [Route("api/lookups/geo")]
    [ResponseCache(CacheProfileName = "CacheLong")]
    public class GeoController : Controller
    {
        [Route("FromMaidenhead")]
        [HttpGet]
        public IActionResult FromMaidenhead(string grid)
        {
            if (MaidenheadLocator.ValidateMaidenhead(grid.ToUpper()))
            {
                var result = MaidenheadLocator.FromMaidenhead(grid);
                return Ok(new {Latitude = result.latitude,Longitude=result.longitude});
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
