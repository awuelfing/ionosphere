using DxLib.DbCaching;
using DXLib.RBN;
using Microsoft.AspNetCore.Mvc;

namespace DXws.Controllers
{
    [Route("/api/spots")]
    [ApiController]
    public class SpotController : Controller
    {
        private readonly DbSpots _dbSpots;
        public SpotController(DbSpots dbSpots)
        {
            _dbSpots = dbSpots;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> PostSpot(Spot spot)
        {
            await _dbSpots.StoreOneAsync(spot);
            return Ok();
        }
        [HttpGet]
        [Route("DeleteAll")]
        public async Task<IActionResult> DeleteAll()
        {
            await _dbSpots.DeleteAllAsync();
            return NoContent(); 
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<IEnumerable<Spot>>> GetAll(string callsign)
        {
            return await _dbSpots.GetAllSpotsAsync(callsign);
        }
    }
}
