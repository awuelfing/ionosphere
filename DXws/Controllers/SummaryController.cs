using DxLib.DbCaching;
using DXLib.RBN;
using Microsoft.AspNetCore.Mvc;

namespace DXws.Controllers
{
    [ApiController]
    [Route("/api/spot/")]
    public class SummaryController : Controller
    {
        private readonly ILogger<SummaryController> _logger;
        private readonly DbSummary _dbSummary;

        public SummaryController(ILogger<SummaryController> logger,DbSummary dbSummary)
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
    }
}
