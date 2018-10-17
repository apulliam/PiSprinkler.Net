using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace SprinklerDotNet.Config
{
    public class CycleProgram
    {
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

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public DayOfWeek[] DaysOfWeek
        {
            get;
            set;
        }

        public int StartHour
        {
            get;
            set;
        }

        public int StartMinute
        {
            get;
            set;
        }

        public ZoneProgram[] Zones
        {
            get;
            set;
        }

        public CycleProgram(string name = null) : this(Guid.Empty, name)
        {
        }

        [JsonConstructor]
        public CycleProgram(Guid id, string name = null)
        {
            if (id == Guid.Empty)
                Id = Guid.NewGuid();
            else
                Id = id;
            Name = name;
        }
    }
}
