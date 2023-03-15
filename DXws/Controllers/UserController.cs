using DxLib.DbCaching;
using DXLib.Cohort;
using DXLib.User;
using DXws.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DXws.Controllers
{
    [ApiController]
    [Route("/api/user")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly DbUser _dbUser;
        public UserController(ILogger<UserController> logger,DbUser dbUser)
        {
            _logger = logger;
            _dbUser = dbUser;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _dbUser.GetUser(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Post(UserRecord userRecord)
        {
            var existing = await _dbUser.GetUser(userRecord.Username);
            if(existing != null)
            {
                return BadRequest();
            }
            await _dbUser.BaseStoreOneAsync(userRecord);
            return CreatedAtAction(nameof(Get), userRecord);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, UserRecord userRecord)
        {
            if (userRecord.Username != id)
            {
                return BadRequest();
            }
            await _dbUser.Update(userRecord);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _dbUser.Delete(id);
            return NoContent();
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> PartialUpdate(string id,JsonPatchDocument<UserRecord> jsonPatch)
        {
            var user = await _dbUser.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }
            jsonPatch.ApplyTo(user);
            await _dbUser.Update(user);
            return NoContent();
        }
    }
}
