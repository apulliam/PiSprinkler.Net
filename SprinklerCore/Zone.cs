namespace SprinklerCore
{

    public class Zone : WeeklyRange
    {
        public int ZoneId
        {
            get;
            private set;
        }

        public int RunTime
        {
            get;
            set;
        }

        internal Zone(int zoneId, int startMinuteOfMonth, int runTime)
        {
            ZoneId = zoneId;
            RunTime = runTime;
            StartMinuteOfWeek = startMinuteOfMonth;
            EndMinuteOfWeek = StartMinuteOfWeek + RunTime;
        }
    }
}
