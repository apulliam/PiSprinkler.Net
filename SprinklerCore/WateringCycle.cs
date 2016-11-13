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

        private int _cycleLength = 0;

        public int CycleLength
        {
            get
            {
                return _cycleLength;
            }
        }
      
        public WateringCycle(CycleConfig cycleConfig)
        {
            DayOfWeek = cycleConfig.DayOfWeek;
            StartHour = cycleConfig.StartHour;
            StartMinute = cycleConfig.StartMinute;
            CycleId = Guid.NewGuid();

            StartMinuteOfWeek = ToMinuteOfWeek(DayOfWeek, StartHour, StartMinute);

            foreach (var zoneConfig in cycleConfig.ZoneConfigs)
            {
                var zone = new Zone(zoneConfig.ZoneNumber, StartMinuteOfWeek + CycleLength, zoneConfig.Time);
                _cycleLength += zoneConfig.Time;
                Zones.Add(zone);
            }
         
            EndMinuteOfWeek = StartMinuteOfWeek + CycleLength;
          
        }

        private bool RangeOverlaps(int x1, int x2, int y1, int y2)
        {
            return (x1 <= y2 && y1 <= x2);
        }

        internal bool ConflictsWith(WateringCycle cycle)
        {
            if (RangeOverlaps(StartMinuteOfWeek, EndMinuteOfWeek, cycle.StartMinuteOfWeek, cycle.EndMinuteOfWeek))
                return true;
            if (RangeOverlaps(StartMinuteOfWeek, EndMinuteOfWeek, 0, cycle.OverlapMinuteOfWeek))
                return true;
            if (RangeOverlaps(0, OverlapMinuteOfWeek, cycle.StartMinuteOfWeek, cycle.EndMinuteOfWeek))
                return true;
            if (OverlapMinuteOfWeek > 0 && cycle.OverlapMinuteOfWeek > 0)
                return true;
            return false;
        }

        internal static int ToMinuteOfWeek(DayOfWeek dayOfWeek, int startHour, int startMinute)
        {
            return ((int)dayOfWeek * 24 * 60) + (startHour * 60) + startMinute;
        }
    }
}
