using DxLib.DbCaching;
using DXLib.RBN;
using DXws.Admin;
using DXws.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;

namespace DXws.Controllers
{
    [Route("/api/spots")]
    [ApiController]
    [Authorize(Roles = "Read")]
    public class SpotController : Controller
    {
        private readonly ILogger<SpotController> _logger;
        private readonly DbSpots _dbSpots;
        private readonly DbCohort _dbCohort;
        public SpotController(ILogger<SpotController> logger, DbSpots dbSpots,DbCohort dbCohort)
        {
            _dbSpots = dbSpots;
            _dbCohort = dbCohort;
            _logger = logger;
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
            await _dbSpots.BaseStoreOneAsync(spot);
            return Ok();
        }
        [HttpGet]
        [Route("DeleteAll")]
        public async Task<IActionResult> DeleteAll()
        {
            _logger.Log(LogLevel.Information, "Purging all spots");
            await _dbSpots.BaseDeleteAllAsync();
            return NoContent(); 
        }
        [HttpGet]
        [Route("DeleteOld")]
        public async Task<IActionResult> DeleteOld(int minutes = 6*60)
        {
            _logger.Log(LogLevel.Information, "Purging spots > {minutes}m", minutes);
            var filter = Builders<Spot>.Filter.Lt("ReceivedDateTime", DateTime.UtcNow.AddHours(-minutes));
            await _dbSpots.BaseDeleteAsync(filter);
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
        [HttpGet]
        [Route("GetAllCohortSpotsDetailed")]
        public async Task<ActionResult> GetAllCohortSpotsDetailed(string Username)
        {
            Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
            var cohorts = await _dbCohort.Get(Username);
            if (cohorts == null) return NotFound();

            var spots = await _dbSpots.GetAllCohortSpotsAsync(cohorts.Cohorts.ToArray(), 10);
            foreach (string call in spots.DistinctBy(x => x.Spottee).Select(x => x.Spottee))
            {
                //var bands = spots.Where(x => x.Spottee.Contains(call)).DistinctBy(x => x.Band).Select(x => x.Band);
                var bands = spots.Where(x => x.Spottee.StartsWith(call)).Select(x => $"{x.Band}[{x.SpotterStationInfo?.Continent ?? string.Empty}]").Distinct();
                results.Add(call, bands.ToList());
            }

            return Ok(results);
        }
        [HttpGet]
        [Route("GetAllCohortSpotsByBand")]
        public async Task<ActionResult> GetAllCohortSpotsByBand(string Username)
        {
            Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
            var cohorts = await _dbCohort.Get(Username);
            if (cohorts == null) return NotFound();

            var spots = await _dbSpots.GetAllCohortSpotsAsync(cohorts.Cohorts.ToArray(), 10);
            foreach (string band in spots.DistinctBy(x => x.Band).Select(x => x.Band))
            {
                var calls = spots.Where(x => x.Band.Equals(band)).DistinctBy(x => x.Spottee).Select(x=>x.Spottee);
                results.Add(band, calls.ToList());
            }

            return Ok(results);
        }
        [HttpGet]
        [Route("GetAllCohortSpotsByBandComplex")]
        public async Task<ActionResult> GetAllCohortSpotsByBandComplex(string Username)
        {
            List<BandModel> newResult = new List<BandModel>();
            var cohorts = await _dbCohort.Get(Username);
            if (cohorts == null) return NotFound();

            var spots = await _dbSpots.GetAllCohortSpotsAsync(cohorts.Cohorts.ToArray(), 10);

            var bands = spots.GroupBy(x => new { x.Band, x.Spottee, x.SpotterStationInfo?.Continent });
            foreach(var band in bands.DistinctBy(x => x.Key.Band))
            {
                BandModel bandModel = new BandModel()
                {
                    Band = band.Key.Band,
                    InnerCall = new List<CallModel>()
                };
                foreach(var call in bands.Where(x=>x.Key.Band == band.Key.Band).DistinctBy(x=>x.Key.Spottee))
                {
                    CallModel callModel = new CallModel()
                    {
                        Call = call.Key.Spottee,
                        InnerContinent = new List<ContinentModel>()
                    };
                    foreach(var cont in bands.Where(x=> x.Key.Band == band.Key.Band && x.Key.Spottee == call.Key.Spottee))
                    {
                        ContinentModel continent = new ContinentModel()
                        {
                            Continent = cont.Key.Continent!,
                            Count = cont.Count()
                        };
                        callModel.InnerContinent.Add(continent);
                    }
                    bandModel.InnerCall.Add(callModel);
                }
                newResult.Add(bandModel);
            }

            return Ok(newResult);
        }
        [HttpGet]
        [Route("GetAllCohortSpotsByBandComplexView")]
        public async Task<ActionResult> GetAllCohortSpotsByBandComplexView(string? input = "")
        {
            string username;
            if (string.IsNullOrEmpty(input))
            {
                username = AdminHelp.GetUser(HttpContext.User.Claims);
            }
            else username = input;

            List<BandModel> newResult = new List<BandModel>();
            var cohorts = await _dbCohort.Get(username);
            if (cohorts == null) return NotFound();

            var spots = await _dbSpots.GetAllCohortSpotsAsync(cohorts.Cohorts.ToArray(), 9999);

            var bands = spots.GroupBy(x => new { x.Band, x.Spottee, x.SpotterStationInfo?.Continent });
            foreach (var band in bands.DistinctBy(x => x.Key.Band))
            {
                BandModel bandModel = new BandModel()
                {
                    Band = band.Key.Band,
                    InnerCall = new List<CallModel>()
                };
                foreach (var call in bands.Where(x => x.Key.Band == band.Key.Band).DistinctBy(x => x.Key.Spottee))
                {
                    CallModel callModel = new CallModel()
                    {
                        Call = call.Key.Spottee,
                        InnerContinent = new List<ContinentModel>()
                    };
                    foreach (var cont in bands.Where(x => x.Key.Band == band.Key.Band && x.Key.Spottee == call.Key.Spottee))
                    {
                        ContinentModel continent = new ContinentModel()
                        {
                            Continent = cont.Key.Continent!,
                            Count = cont.Count()
                        };
                        callModel.InnerContinent.Add(continent);
                    }
                    bandModel.InnerCall.Add(callModel);
                }
                newResult.Add(bandModel);
            }

            return View("View",newResult);
        }
    }
}
