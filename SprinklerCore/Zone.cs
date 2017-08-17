using Newtonsoft.Json;

namespace SprinklerCore
{
    public sealed class Zone : WeeklyRange
    {
        public int Id
        {
            get;
            private set;
        }

        internal Zone(int zoneId, int startMinuteOfWeek, int runTime)
        {
            Id = zoneId;
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

        [JsonConstructor]
        internal Zone(int ZoneId, int StartMinuteOfWeek, int RunTime, int EndMinuteOfWeek)
        {
            this.Id = ZoneId;
            this.RunTime = RunTime;
            this.StartMinuteOfWeek = StartMinuteOfWeek;
            this.EndMinuteOfWeek = EndMinuteOfWeek;

        }
    }
}
