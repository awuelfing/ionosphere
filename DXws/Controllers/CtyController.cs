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
        public ActionResult<CtyResult?> Get(string callsign)
        {
            _logger.Log(LogLevel.Debug, "CtyLookup for {callsign} called", callsign);
            try
            {
                CtyResult? result = Cty.MatchCall(callsign);
                if(result == null)
                {
                    _logger.Log(LogLevel.Warning, "CtyLookup for {callsign} failed", callsign);
                    return BadRequest();
                }
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error,ex ,"CtyLookup for {callsign} failed", callsign);
                return BadRequest();
            }
        }

    }
}
