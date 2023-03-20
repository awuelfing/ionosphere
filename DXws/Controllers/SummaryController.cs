using DxLib.DbCaching;
using DXLib.RBN;
using DXLib.Reference;
using Microsoft.AspNetCore.Mvc;

namespace DXws.Controllers
{
    [ApiController]
    [Route("/api/spot/")]
    public class SummaryController : Controller
    {
        private readonly ILogger<SummaryController> _logger;
        private readonly DbSummary _dbSummary;

        public SummaryController(ILogger<SummaryController> logger, DbSummary dbSummary)
        {
            _logger = logger;
            _dbSummary = dbSummary;
        }

        [HttpPost]
        [Route("Summary")]
        public async Task<IActionResult> Post(SummaryRecord summaryRecord)
        {
            await _dbSummary.BaseStoreOneAsync(summaryRecord);
            return Ok();
        }
        [HttpGet]
        [Route("Summary")]
        [ResponseCache(CacheProfileName = "CacheShort")]
        public async Task<ActionResult<SummaryRecord>> GetMostRecentSummary()
        {
            return await _dbSummary.GetMostRecentSummaryAsync();
        }
        [HttpGet]
        [Route("SummaryReverse")]
        [ResponseCache(CacheProfileName = "CacheShort")]
        public async Task<IActionResult> GetMostRecentSummaryReverse()
        {
            var summary = await _dbSummary.GetMostRecentSummaryAsync();
            var filteredSummary = summary.Activity!.Where(x => !x.Key.EndsWith("/B"));

            Dictionary<string, int> result = new Dictionary<string, int>();
            
            foreach (string s in Helper.Bands)
            {
                result.Add(s, filteredSummary.Where(x => x.Value.Contains(s)).Count());
            }
            return Ok(result);
        }
    }
}
