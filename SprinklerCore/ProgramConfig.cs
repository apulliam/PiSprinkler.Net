
namespace SprinklerCore
{
    public class ProgramConfig
    {
        public string ProgramName { get; private set; }

        public CycleConfig[] CycleConfigs { get; private set; }

        public ProgramConfig(string programName, CycleConfig[] cycleConfigs)
        {
            ProgramName = programName;
            CycleConfigs = cycleConfigs;
        }
    }
}
