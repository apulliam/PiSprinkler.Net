using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace SprinklerCore
{
    public sealed class CycleConfig
    {
        [JsonConverter(typeof(StringEnumConverter))]
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

        public ZoneConfig[] Zones
        {
            get;
            set;
        }

        [JsonConstructor]
        public CycleConfig()
        {
        }
        
    }
}
