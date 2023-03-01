using DxLib.DbCaching;
using DXLib.Cohort;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace DXws.Controllers.Web
{
    [Route("/web/cohort")]
    public class WebCohortController : Controller
    {
        private readonly ILogger<WebCohortController> _logger;
        private readonly DbCohort _dbCohort;
        public WebCohortController(ILogger<WebCohortController> logger, DbCohort dbCohort)
        {
            _logger = logger;
            _dbCohort = dbCohort;
        }
        [HttpPost]
        [Route("AppendOne")]
        public async Task<IActionResult> AppendOne([FromForm]string Username, [FromForm] string Callsign)
        {
            CohortRecord? cohortRecord = await _dbCohort.Get(Username);
            if (cohortRecord == null)
            {
                return BadRequest();
            }
            if (cohortRecord.Cohorts.Contains(Callsign))
            {
                return Ok();
            }
            cohortRecord.Cohorts = cohortRecord.Cohorts.Append(Callsign).ToList();
            await _dbCohort.Update(cohortRecord);
            return RedirectToAction("Edit", "WebCohort");
        }
        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit()
        {
            var cohort = await _dbCohort.Get("Admin");
            return View("EditCohort", cohort);
        }
    }
}
