using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace SprinklerCore
{
    public class WateringCycle : WeeklyRange
    {
        internal Program Program
        {
            get;
            private set;
        }

        public Guid Id
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

        private List<ZoneCycle> _zones = new List<ZoneCycle>();

        public List<ZoneCycle> Zones
        {
            get
            {
                return _zones;
            }
        }

        internal static List<WateringCycle> ToWateringCycles(Program program, CycleConfig cycleConfig)
        {
            var cycles = new List<WateringCycle>();
            foreach (var day in cycleConfig.DaysOfWeek)
            {
                cycles.Add(new WateringCycle(program, day, cycleConfig.StartHour, cycleConfig.StartMinute, cycleConfig.ZoneConfigs));
            }
            return cycles;
        }

        internal WateringCycle(Program program, DayOfWeek dayOfWeek, int startHour, int startMinute, ZoneConfig[] zoneConfigs)
        {
            Program = program;
            DayOfWeek = dayOfWeek;
            StartHour = startHour;
            StartMinute = startMinute;
            Id = Guid.NewGuid();

            StartMinuteOfWeek = ToMinuteOfWeek(DayOfWeek, StartHour, StartMinute);

            var runTime = 0;
            foreach (var zoneConfig in zoneConfigs)
            {
                var zone = new ZoneCycle(zoneConfig.ZoneNumber, StartMinuteOfWeek + runTime, zoneConfig.Time);
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
        internal WateringCycle(Guid CycleId, DayOfWeek DayOfWeek, int StartHour, int StartMinute, IEnumerable<ZoneCycle> Zones, int StartMinuteOfWeek, int RunTime, int EndMinuteOfWeek)
        {
            this.Id = CycleId;
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
