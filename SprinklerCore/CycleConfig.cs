using System;

namespace SprinklerCore
{
    public sealed class CycleConfig
    {
        public string Name
        {
            get;
            set;
        }

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

        public ZoneConfig[] ZoneConfigs
        {
            get;
            set;
        }

        
    }
}
