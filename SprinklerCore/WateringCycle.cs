using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace SprinklerCore
{
    public class WateringCycle : WeeklyRange
    {
        //public string Program
        //{
        //    get
        //    {
        //        return Parent.Name;
        //    }
        //}

        internal Program Parent
        {
            get;
            set;
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

        private List<Zone> _zones = new List<Zone>();

        public List<Zone> Zones
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
            Parent = program;
            DayOfWeek = dayOfWeek;
            StartHour = startHour;
            StartMinute = startMinute;
            Id = Guid.NewGuid();

            StartMinuteOfWeek = ToMinuteOfWeek(DayOfWeek, StartHour, StartMinute);

            var runTime = 0;
            foreach (var zoneConfig in zoneConfigs)
            {
                var zone = new Zone(zoneConfig.ZoneNumber, StartMinuteOfWeek + runTime, zoneConfig.RunTime);
                runTime += zoneConfig.RunTime;
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
        internal WateringCycle(Guid Id, DayOfWeek DayOfWeek, int StartHour, int StartMinute, IEnumerable<Zone> Zones, int StartMinuteOfWeek, int RunTime, int EndMinuteOfWeek)
        {
            this.Id = Id;
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
