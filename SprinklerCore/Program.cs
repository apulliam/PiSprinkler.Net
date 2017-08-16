using System;
using System.Collections.Generic;

namespace SprinklerCore
{
    public class Program
    {
        private List<WateringCycle> _cycles = new List<WateringCycle>();

        public Guid Id
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public List<WateringCycle> Cycles
        {
            get
            {
                return _cycles;
            }
        }

        internal Program(ProgramConfig programConfig)
        {
            Id = Guid.NewGuid();
            Name = programConfig.ProgramName;
            foreach (var config in programConfig.CycleConfigs)
            {
                _cycles.AddRange(WateringCycle.ToWateringCycles(this, config));
            }
        }
    }
}
