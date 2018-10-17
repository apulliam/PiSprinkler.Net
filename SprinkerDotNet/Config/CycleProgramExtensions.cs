using SprinklerDotNet.Config;
using SprinklerDotNet.Runtime;
using System.Collections.Generic;

namespace SprinklerDotNet.Config
{
    public static class CycleProgramExtensions
    {
        public static List<WateringCycle> ToWateringCycles(this CycleProgram cycleProgram)
        {
            var cycles = new List<WateringCycle>();

            foreach (var day in cycleProgram.DaysOfWeek)
            {
                cycles.Add(new WateringCycle(cycleProgram, day, cycleProgram.StartHour, cycleProgram.StartMinute, cycleProgram.Zones));
            }
            return cycles;
        }
    }
}
