using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SprinklerCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PiSprinkler.Controllers
{
    [Route("api/[controller]")]
    public class ZonesController : Controller
    {
        private SprinklerController SprinklerController { get; set; }

        public ZonesController(SprinklerController sprinklerController)
        {
            SprinklerController = sprinklerController;
        }

        [HttpGet("zones")]
        public IEnumerable<ZoneController> GetZones()
        {
            return  SprinklerController.GetAllZones();
        }

        [HttpGet("zones/{id}")]
        public ZoneController GetZone(string id)
        {
            return SprinklerController.GetZone(int.Parse(id));
        }

        [HttpPost("zones/{id}/start")]
        public void StartZone(string id)
        {
            SprinklerController.StartZone(int.Parse(id));
        }

        [HttpPost("zones/stop")]
        public void StopZone()
        {
            SprinklerController.StopZone();
        }
    }
}
