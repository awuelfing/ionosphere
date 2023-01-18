using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DXLib.CtyDat;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace DXws.Controllers
{
    [Route("api/lookups")]
    [ApiController]
    public class LookupController : ControllerBase
    {
        private readonly ILogger<LookupController> _logger;

        public LookupController(ILogger<LookupController> logger)
        {
            _logger = logger;
        }
        [HttpGet(Name = "CtyLookup")]
        public ActionResult<CtyResult> Get(string callsign)
        {
            try
            {
                return Cty.MatchCall(callsign);
            }
            catch
            {
                return BadRequest();
            }
        }

    }
}
