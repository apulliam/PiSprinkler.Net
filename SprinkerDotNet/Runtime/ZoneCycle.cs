using Newtonsoft.Json;

namespace SprinklerDotNet.Runtime
{
    public class ZoneCycle : WeeklyRange
    {
        public int ZoneNumber
        {
            get;
            private set;
        }

        internal ZoneCycle(int zoneNumber, int startMinuteOfWeek, int runTime)
        {
            ZoneNumber = zoneNumber;
            RunTime = runTime;

            if (startMinuteOfWeek >= 10080)
            {
                StartMinuteOfWeek = startMinuteOfWeek - 10080;
            }
            else
                StartMinuteOfWeek = startMinuteOfWeek;

            var endMinuteOfWeek = startMinuteOfWeek + RunTime;
            if (endMinuteOfWeek >= 10080)
            {
                EndMinuteOfWeek = endMinuteOfWeek - 10080;
            }
            else
                EndMinuteOfWeek = endMinuteOfWeek;
        }
    }
}
