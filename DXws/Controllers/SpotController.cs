using DxLib.DbCaching;
using DXLib.RBN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Data;

namespace DXws.Controllers
{
    [Route("/api/spots")]
    [ApiController]
    [Authorize(Roles = "Read")]
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
        [Route("DeleteOld")]
        public async Task<IActionResult> DeleteOld()
        {
            var filter = Builders<Spot>.Filter.Gt("ReceivedDateTime", DateTime.UtcNow.AddHours(-6));
            await _dbSpots.DeleteAsync(filter);
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

            var spots = await _dbSpots.GetAllCohortSpotsAsync(cohorts.Cohorts.ToArray(),10);
            foreach(string call in spots.DistinctBy(x=>x.Spottee).Select(x=>x.Spottee))
            {
                var bands = spots.Where(x => x.Spottee.Contains(call)).DistinctBy(x => x.Band).Select(x => x.Band);
                results.Add(call, bands.ToList());
            }

            return Ok(results);
        }
    }
}
