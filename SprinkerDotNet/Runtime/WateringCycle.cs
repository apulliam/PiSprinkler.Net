using System;
using System.Collections.Generic;
using SprinklerDotNet.Config;

namespace SprinklerDotNet.Runtime
{
    public class WateringCycle : WeeklyRange
    {
        internal CycleProgram Program
        {
            get;
            set;
        }

        public Guid Id
        {
            get;
            private set;
        }
              
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

        internal WateringCycle(CycleProgram cycleConfig, DayOfWeek dayOfWeek, int startHour, int startMinute, ZoneProgram[] zoneConfigs)
        {
            Program = cycleConfig;
            DayOfWeek = dayOfWeek;
            StartHour = startHour;
            StartMinute = startMinute;
            Id = Guid.NewGuid();

            StartMinuteOfWeek = ToMinuteOfWeek(DayOfWeek, StartHour, StartMinute);

            var runTime = 0;
            foreach (var zoneConfig in zoneConfigs)
            {
                var zone = new ZoneCycle(zoneConfig.ZoneNumber, StartMinuteOfWeek + runTime, zoneConfig.RunTime);
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

       
    }
}
