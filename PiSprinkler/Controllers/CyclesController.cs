using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SprinklerCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PiSprinkler.Controllers
{
    [Route("api/[controller]")]
    public class CyclesController : Controller
    {
        private SprinklerController SprinklerController { get; set; }

        public CyclesController(SprinklerController sprinklerController)
        {
            SprinklerController = sprinklerController;
        }
     
        [HttpGet]
        public IEnumerable<WateringCycle> GetAllWateringCycles()
        {
            return SprinklerController.GetAllWateringCycles();
        }

        [HttpGet("{zoneNumber}")]
        public IEnumerable<WateringCycle> GetWateringCyclesByZone(string zoneNumber)
        {
            return SprinklerController.GetWateringCyclesByZone(int.Parse(zoneNumber));
        }


    }
}
