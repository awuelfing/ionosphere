using Microsoft.AspNetCore.Mvc;
using DXLib;
using DXLib.HamQTH;
using System.Text.Json;

namespace DXws.Controllers
{
    [Route("api/lookups/HamQTH")]
    public class HamQTHController : Controller
    {
        public HamQTHController()
        {
            
        }
        [HttpGet]
        public async Task<ActionResult> Get(string callsign)
        {
            HamQTHGeo hamQTHGeo = new();
            HamQTHResult hamQTHResult = await hamQTHGeo.GetGeo(callsign);
            string serialized = JsonSerializer.Serialize(hamQTHResult, new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
            return Ok(serialized);
        }
    }
}
