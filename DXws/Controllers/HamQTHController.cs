using DxLib.DbCaching;
using Microsoft.AspNetCore.Mvc;
using DXLib;
using DXLib.HamQTH;
using System.Text.Json;


namespace DXws.Controllers
{
    [Route("api/lookups/HamQTH")]
    public class HamQTHController : Controller
    {
        private QthLookup _QthLookup;
        public HamQTHController(QthLookup qthLookup)
        {
            _QthLookup = qthLookup;
        }
        [HttpGet]
        public async Task<ActionResult> Get(string callsign)
        {
            HamQTHResult? hamQTHResult = await _QthLookup.GetGeoAsync(callsign);
            HamQTHSearchResult? hamQTHSearchResult = hamQTHResult!.SearchResult;
            if (hamQTHSearchResult != null)
            {
                string serialized = JsonSerializer.Serialize(hamQTHSearchResult, new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = true
                });
                return Ok(serialized);
            }
            else
            {
                return NotFound();
            }
            
             
        }
    }
}
