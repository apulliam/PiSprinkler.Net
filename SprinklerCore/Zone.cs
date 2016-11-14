using Newtonsoft.Json;

namespace SprinklerCore
{
    public class Zone : WeeklyRange
    {
        public int ZoneId
        {
            get;
            private set;
        }
       
        internal Zone(int zoneId, int startMinuteOfWeek, int runTime)
        {
            ZoneId = zoneId;
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

        internal Zone()
        {
        }

        [JsonConstructor]
        internal Zone(int ZoneId, int StartMinuteOfWeek, int RunTime, int EndMinuteOfWeek)
        {
            this.ZoneId = ZoneId;
            this.RunTime = RunTime;
            this.StartMinuteOfWeek = StartMinuteOfWeek;
            this.EndMinuteOfWeek = EndMinuteOfWeek;
            
        }
    }
}
