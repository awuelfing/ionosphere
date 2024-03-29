﻿using DxLib.DbCaching;
using Microsoft.AspNetCore.Mvc;
using DXLib;
using DXLib.HamQTH;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace DXws.Controllers
{
    [Route("api/lookups/HamQTH")]
    [Authorize(Roles = "Read")]
    public class HamQTHController : Controller
    {
        private IQthLookup _QthLookup;
        private DbQueue _dbQueue;
        public HamQTHController(IQthLookup qthLookup, DbQueue dbQueue)
        {
            _QthLookup = qthLookup;
            _dbQueue = dbQueue;
        }
        [HttpGet]
        public async Task<ActionResult> Get(string callsign)
        {
            HamQTHResult? hamQTHResult = await _QthLookup.GetGeoAsync(callsign);
            HamQTHSearchResult? hamQTHSearchResult = hamQTHResult!.SearchResult;
            if (hamQTHSearchResult != null)
            {
                string serialized = JsonSerializer.Serialize(hamQTHSearchResult, new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = true
                });
                return Ok(serialized);
            }
            else
            {
                return NoContent();
            }
        }
        [HttpGet]
        [Route("GetFullRecord")]
        public async Task<ActionResult> GetFullRecord(string callsign,bool LookLower=true)
        {
            HamQTHResult? hamQTHResult = await _QthLookup.GetGeoAsync(callsign,LookLower);
            if (hamQTHResult != null)
            {
                string serialized = JsonSerializer.Serialize(hamQTHResult, new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = true
                });
                return Ok(serialized);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet]
        [Route("Enqueue")]
        public async Task<ActionResult> Queue(string callsign,int count)
        {
            await _dbQueue.EnqueueAsync(callsign,count);
            return Ok();
        }
        [HttpGet]
        [Route("Dequeue")]
        public async Task<ActionResult> Dequeue()
        {
            var result = await _dbQueue.DequeueAsync(false);
            if(result!= null) return Ok(result);
            else return NoContent();
        }
        [HttpGet]
        [Route("Peek")]
        public async Task<ActionResult> Peek()
        {
            var result = await _dbQueue.DequeueAsync(true);
            if (result != null) return Ok(result);
            else return NoContent();
        }
    }
}
