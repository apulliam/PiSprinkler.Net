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
      
        public WateringCycle(DayOfWeek dayOfWeek, int startHour, int startMinute, int[][] zoneTimes)
        {
            DayOfWeek = dayOfWeek;
            StartHour = startHour;
            StartMinute = startMinute;
            CycleId = Guid.NewGuid();

            StartMinuteOfWeek = ToMinuteOfWeek(DayOfWeek, StartHour, StartMinute);

            for (int i = 0; i < zoneTimes.Length; i++)
            {
                var zone = new Zone(zoneTimes[i][0], StartMinuteOfWeek + CycleLength, zoneTimes[i][1]);
                _cycleLength += zoneTimes[i][1];
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
