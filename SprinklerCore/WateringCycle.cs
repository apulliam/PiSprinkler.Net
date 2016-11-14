using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace SprinklerCore
{
    public class WateringCycle : WeeklyRange
    {
       
        public Guid CycleId
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }


        [JsonConverter(typeof(StringEnumConverter))]
        public DayOfWeek DayOfWeek
        {
            get;
            private set;
        }

        public int StartHour
        {
            get;
            private set;
        }

        public int StartMinute
        {
            get;
            private set;
        }

        private List<Zone> _zones = new List<Zone>();

        public List<Zone> Zones
        {
            get
            {
                return _zones;
            }
        }

            
        internal WateringCycle(CycleConfig cycleConfig)
        {
            Name = cycleConfig.Name;
            DayOfWeek = cycleConfig.DayOfWeek;
            StartHour = cycleConfig.StartHour;
            StartMinute = cycleConfig.StartMinute;
            CycleId = Guid.NewGuid();

           StartMinuteOfWeek = ToMinuteOfWeek(DayOfWeek, StartHour, StartMinute);

            var runTime = 0;
            foreach (var zoneConfig in cycleConfig.ZoneConfigs)
            {
                var zone = new Zone(zoneConfig.ZoneNumber, StartMinuteOfWeek + runTime, zoneConfig.Time);
                runTime += zoneConfig.Time;
                Zones.Add(zone);
            }
            RunTime = runTime;
            var endMinuteOfWeek = StartMinuteOfWeek + RunTime;
            if (endMinuteOfWeek >= 10080)
            {
                EndMinuteOfWeek = endMinuteOfWeek - 10080;
            }
            else
                EndMinuteOfWeek = endMinuteOfWeek;
        }

        [JsonConstructor]
        internal WateringCycle(Guid CycleId, string Name, DayOfWeek DayOfWeek, int StartHour, int StartMinute, IEnumerable<Zone> Zones, int StartMinuteOfWeek, int RunTime, int EndMinuteOfWeek)
        {
            this.CycleId = CycleId;
            this.Name = Name;
            this.DayOfWeek = DayOfWeek;
            this.StartHour = StartHour;
            this.StartMinute = StartMinute;
            this.StartMinuteOfWeek = StartMinuteOfWeek;
            this.EndMinuteOfWeek = EndMinuteOfWeek;
            this.RunTime = RunTime;
            foreach (var zone in Zones)
                _zones.Add(zone);
        }

       
    }
}
