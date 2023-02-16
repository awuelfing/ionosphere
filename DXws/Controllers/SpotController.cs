using DxLib.DbCaching;
using DXLib.RBN;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace DXws.Controllers
{
    [Route("/api/spots")]
    [ApiController]
    public class SpotController : Controller
    {
        private readonly DbSpots _dbSpots;
        private readonly DbCohort _dbCohort;
        public SpotController(DbSpots dbSpots,DbCohort dbCohort)
        {
            _dbSpots = dbSpots;
            _dbCohort = dbCohort;
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
        [HttpGet]
        [Route("GetAllCohortSpots")]
        public async Task<ActionResult> GetAllCohortSpots(string Username)
        {
            Dictionary<string,List<string>> results = new Dictionary<string, List<string>>();
            var cohorts = await _dbCohort.Get(Username);
            if (cohorts == null) return NotFound();

            foreach(string s in cohorts!.Cohorts)
            {
                var spots = await _dbSpots.GetAllSpotsAsync(s);
                var bands = spots.DistinctBy(x => x.Band).Select(x => x.Band).ToList();
                if (bands.Count > 0)
                {
                    results.Add(s, bands);
                }
            }


            return Ok(results);
        }
    }
}
