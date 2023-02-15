using DxLib.DbCaching;
using DXLib.Cohort;
using DXws.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;


namespace DXws.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize(Roles ="Write")]
    public class CohortController : Controller
    {
        private readonly DbCohort _dbCohort;
        public CohortController(DbCohort dbCohort)
        {
            _dbCohort = dbCohort;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string username)
        {
            var result = await _dbCohort.Get(username);
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
            await _dbCohort.StoreOneAsync(cohortRecord);
            return CreatedAtAction(nameof(Post), cohortRecord);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, CohortRecord cohortRecord)
        {
            if(cohortRecord.Username != id)
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
    }
}
