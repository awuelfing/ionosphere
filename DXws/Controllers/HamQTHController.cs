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
        private DbQueue _dbQueue;
        public HamQTHController(QthLookup qthLookup, DbQueue dbQueue)
        {
            _QthLookup = qthLookup;
            _dbQueue = dbQueue;
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
        [HttpGet]
        [Route("Queue")]
        public async Task<ActionResult> Queue(string callsign)
        {
            await _dbQueue.EnqueueAsync(callsign);
            return Ok();
        }
    }
}
