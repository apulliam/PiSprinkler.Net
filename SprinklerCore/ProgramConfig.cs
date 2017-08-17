
namespace SprinklerCore
{
    public class ProgramConfig
    {
        public string Name { get; private set; }

        public CycleConfig[] CycleConfigs { get; private set; }

        public ProgramConfig(string name, CycleConfig[] cycleConfigs)
        {
            Name = name;
            CycleConfigs = cycleConfigs;
        }
    }
}
