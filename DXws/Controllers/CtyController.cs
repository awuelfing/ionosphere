using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DXLib.CtyDat;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace DXws.Controllers
{
    [Route("api/lookups/cty")]
    [ApiController]
    public class CtyController : ControllerBase
    {
        private readonly ILogger<CtyController> _logger;

        public CtyController(ILogger<CtyController> logger)
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
