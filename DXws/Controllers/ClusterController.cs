using DxLib.DbCaching;
using DXLib.ClusterList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DXws.Controllers
{
    [ApiController]
    [Route("/api/cluster")]
    public class ClusterController : Controller
    {
        private readonly ILogger<ClusterController> _logger;
        private readonly DbCluster _dbCluster;
        public ClusterController(ILogger<ClusterController> logger,DbCluster dbCluster)
        {
            _logger = logger;
            _dbCluster = dbCluster;
        }
        [HttpGet]
        public async Task<ActionResult<ClusterRecord>> GetMostRecentCluster()
        {
            var result = await _dbCluster.GetMostRecentClusterAsync();
            return Ok(result);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostClusterRoot(ClusterRecord clusterRecord)
        {
            _logger.Log(LogLevel.Information, "Received cluster info");
            await _dbCluster.BaseStoreOneAsync(clusterRecord);
            return Ok();
        }
    }
}
