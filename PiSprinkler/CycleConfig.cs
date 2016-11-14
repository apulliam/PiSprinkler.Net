using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiSprinkler
{
    public sealed class CycleConfig
    {
        public string Name
        {
            get;
            set;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public DayOfWeek DayOfWeek
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

        public IEnumerable<ZoneConfig> ZoneConfigs
        {
            get;
            set;
        }
        public CycleConfig()
        {
        }

    }
}
