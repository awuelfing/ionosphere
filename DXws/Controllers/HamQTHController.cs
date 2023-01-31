using Microsoft.AspNetCore.Mvc;
using DXLib;
using DXLib.HamQTH;

namespace DXws.Controllers
{
    [Route("api/lookups/HamQTH")]
    public class HamQTHController : Controller
    {
        [HttpGet]
        public async Task<ActionResult> Get(string callsign)
        {
            HamQTHGeo hamQTHGeo = new();
            HamQTHResult hamQTHResult = await hamQTHGeo.GetGeo(callsign);
            return Ok(hamQTHResult.SearchResult);
        }
    }
}
