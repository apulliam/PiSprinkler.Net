using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SprinklerDotNet;
using SprinklerDotNet.Config;
using SprinklerDotNet.Runtime;

namespace PiSprinkler.AspNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SprinklerController : ControllerBase
    {
        private SprinklerBase _sprinkler;
        public SprinklerController(SprinklerBase sprinkler)
        {
            _sprinkler = sprinkler;
        }

        [HttpGet("programs")]
        public async Task<IEnumerable<CycleProgram>> GetPrograms()
        {
            return await _sprinkler.GetPrograms();
        }

        [HttpDelete("programs/{id}")]
        public async Task DeleteProgram(string id)
        {
            await _sprinkler.DeleteProgram(Guid.Parse(id));
        }

        [HttpPost("programs")]
        public async Task<ActionResult> AddProgram([FromBody]CycleProgram cycleProgram)
        {
            var programId = await _sprinkler.AddProgram(cycleProgram);
            return new CreatedResult("", programId);
        }

        [HttpDelete("programs")]
        public async Task ClearAllPrograms()
        {
            await _sprinkler.ClearPrograms();
        }

        [HttpGet("programs/{id}")]
        public async Task<CycleProgram> GetProgram(string id)
        {
            return await _sprinkler.GetProgram(Guid.Parse(id));
        }

        [HttpGet("cycles")]
        public async Task<IEnumerable<WateringCycle>> GetWateringCycles()
        {
            return await _sprinkler.GetWateringCycles();
        }

        [HttpGet("cycles/{zoneNumber}")]
        public async Task<IEnumerable<WateringCycle>> GetWateringCyclesByZone(string zoneNumber)
        {
            return await _sprinkler.GetWateringCyclesByZone(int.Parse(zoneNumber));
        }

        [HttpGet("zones")]
        public async Task<IEnumerable<ZoneBase>> GetZones()
        {
            return await _sprinkler.GetZones();
        }

        [HttpGet("zones/{id}")]
        public async Task<ZoneBase> GetZone(string id)
        {
            return await _sprinkler.GetZone(int.Parse(id));
        }

        [HttpPut("zones/{id}/start")]
        public async Task StartZone(string id)
        {
            await _sprinkler.StartZone(int.Parse(id));
        }

        [HttpPut("zones/stop")]
        public async Task StopZone()
        {
            await _sprinkler.StopZone();
        }
    }
}
