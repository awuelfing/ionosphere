using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DXLib.ScpDb;
using DXws.Models;

namespace DXws.Controllers
{
    [Route("api/lookups/scp")]
    [ApiController]
    [ResponseCache(CacheProfileName = "CacheLong")]
    public class ScpController : ControllerBase
    {
        private readonly ILogger<ScpController> _logger;
        public ScpController(ILogger<ScpController> logger) {
            _logger = logger;
        }
        [HttpGet]
        public ActionResult<ScpResult> CheckScp(string callsign)
        {
            return new ScpResult()
            {
                Callsign = callsign,
                Match = Scp.MatchScp(callsign)
            };
        }
    }
}
