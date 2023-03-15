using DxLib.DbCaching;
using DXLib.Cohort;
using DXws.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;


namespace DXws.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize(Roles = "Read")]
    public class CohortController : Controller
    {
        private readonly DbCohort _dbCohort;
        public CohortController(DbCohort dbCohort)
        {
            _dbCohort = dbCohort;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _dbCohort.Get(id);
            if(result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Post(CohortRecord cohortRecord)
        {
            if(AdminHelp.GetUser(HttpContext.User.Claims) != cohortRecord.Username)
            {
                return Unauthorized();
            }
            await _dbCohort.BaseStoreOneAsync(cohortRecord);
            return CreatedAtAction(nameof(Get), cohortRecord);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, CohortRecord cohortRecord)
        {
            if (AdminHelp.GetUser(HttpContext.User.Claims) != cohortRecord.Username)
            {
                return Unauthorized();
            }
            if (cohortRecord.Username != id)
            {
                return BadRequest();
            }
            await _dbCohort.Update(cohortRecord);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _dbCohort.Delete(id);
            return NoContent();
        }
        [HttpPost]
        [Route("AppendOne")]
        public async Task<IActionResult> AppendOne(string Username, string Callsign)
        {
            if (AdminHelp.GetUser(HttpContext.User.Claims) != Username)
            {
                return Unauthorized();
            }
            CohortRecord? cohortRecord = await _dbCohort.Get(Username);
            if (cohortRecord == null)
            {
                return BadRequest();
            }
            if(cohortRecord.Cohorts.Contains(Callsign))
            {
                return Ok();
            }
            cohortRecord.Cohorts = cohortRecord.Cohorts.Append(Callsign).ToList();
            await _dbCohort.Update(cohortRecord);
            return AcceptedAtAction(nameof(Get), cohortRecord);
        }
        [Route("RemoveOne")]
        [HttpDelete]
        //[HttpDelete("{id}")]
        public async Task<IActionResult> RemoveOne(string id)
        {
            var username = AdminHelp.GetUser(HttpContext.User.Claims);
            await _dbCohort.RemoveOne(username,id);
            return NoContent();
        }
        [Route("AddOne")]
        [HttpPut]
        public async Task<IActionResult> AddOne(string id)
        {
            var username = AdminHelp.GetUser(HttpContext.User.Claims);
            await _dbCohort.AddOne(username,id);
            return NoContent();
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> PartialUpdate(string id, JsonPatchDocument<CohortRecord> jsonPatch)
        {
            if (AdminHelp.GetUser(HttpContext.User.Claims) != id)
            {
                return Unauthorized();
            }
            var cohort = await _dbCohort.Get(id);
            if (cohort == null)
            {
                //kiss
                return NoContent();
            }
            jsonPatch.ApplyTo(cohort);
            await _dbCohort.Update(cohort);
            return NoContent();
        }
    }
}
