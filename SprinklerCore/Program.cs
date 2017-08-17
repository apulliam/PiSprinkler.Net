
using Newtonsoft.Json;
using System;

namespace SprinklerCore
{
    public class Program
    {
        public Guid Id
        {
            get;
            private set;
        }

        public string Name { get; private set; }

        public CycleConfig[] Cycles { get; private set; }

        public Program(string name, CycleConfig[] cycleConfigs)
        {
            Id = Guid.NewGuid();
            Name = name;
            Cycles = cycleConfigs;
        }

        [JsonConstructor]
        public Program(Guid id, string name, CycleConfig[] cycleConfigs)
        {
            Id = id;
            Name = name;
            Cycles = cycleConfigs;
        }
    }
}
