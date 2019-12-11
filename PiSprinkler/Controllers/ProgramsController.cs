using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SprinklerCore;

namespace PiSprinkler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgramsController : ControllerBase
    {
        private SprinklerController SprinklerController { get; set; }

        public ProgramsController(SprinklerController sprinklerController)
        {
            SprinklerController = sprinklerController;
        }
      
        [HttpGet]
        public IEnumerable<SprinklerCore.Program> GetAllPrograms()
        {
            return SprinklerController.GetAllPrograms();
        }

        [HttpDelete]
        public void ClearAllPrograms()
        {
            SprinklerController.ClearAllPrograms();
        }

        [HttpGet("{id}")]
        public SprinklerCore.Program GetProgram(string id)
        {
            return SprinklerController.GetProgram(Guid.Parse(id));
        }

        /// <summary>
        /// Make sure the number of parameters in your UriFormat match the parameters in your method and
        /// the names (case sensitive) and order are respected.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<bool> DeleteProgram(string id)
        {
            return await SprinklerController.DeleteProgram(Guid.Parse(id));
        }

        [HttpPost]
        public async Task<Guid> AddProgram([FromBody] SprinklerCore.Program programConfig)
        {
            return await SprinklerController.AddProgram(programConfig);
        }
    }
}
