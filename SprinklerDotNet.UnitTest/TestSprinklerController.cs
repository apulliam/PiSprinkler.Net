using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SprinklerDotNet;
using SprinklerDotNet.Config;

namespace SprinklerDotNet.UnitTest
{
    internal class TestSprinklerController : SprinklerBase
    {
        public TestSprinklerController()
        {

        }

        protected override Task<IEnumerable<ZoneBase>> ReadZoneConfig()
        {
            return Task.FromResult(new List<ZoneBase>().AsEnumerable());
        }

        protected override Task<IEnumerable<CycleProgram>> ReadCyclePrograms()
        {
            return Task.FromResult(new List<CycleProgram>().AsEnumerable());
        }

        protected override Task WriteCyclePrograms(IEnumerable<CycleProgram> programs)
        {
          
            return Task.CompletedTask;
        }
        public override Task StartScheduler()
        {
            throw new NotImplementedException();
        }

        public override Task StopScheduler()
        {
            throw new NotImplementedException();
        }

    }
}
