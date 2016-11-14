using Newtonsoft.Json;
using System;

namespace SprinklerCore
{
    public abstract class WeeklyRange
    {
        [JsonProperty]
        protected int StartMinuteOfWeek
        {
            get;
            set;
        }
       
        public int RunTime
        {
            get;
            protected set;
        }

        [JsonProperty]
        protected int EndMinuteOfWeek
        {
            get;
            set;
        }
        
        private bool RangeOverlaps(int x1, int x2, int y1, int y2)
        {
            return (x1 <= y2 && y1 <= x2);
        }

        internal bool ConflictsWith(WateringCycle cycle)
        {
            if (EndMinuteOfWeek > StartMinuteOfWeek && cycle.EndMinuteOfWeek > cycle.StartMinuteOfWeek)
            {
                return RangeOverlaps(StartMinuteOfWeek, EndMinuteOfWeek, cycle.StartMinuteOfWeek, cycle.EndMinuteOfWeek);
            }
            else if (EndMinuteOfWeek > StartMinuteOfWeek && cycle.EndMinuteOfWeek < cycle.StartMinuteOfWeek)
            {
                return (cycle.EndMinuteOfWeek >= StartMinuteOfWeek || cycle.StartMinuteOfWeek <= EndMinuteOfWeek);
            }
            else if (EndMinuteOfWeek < StartMinuteOfWeek && cycle.EndMinuteOfWeek > cycle.StartMinuteOfWeek)
            {
                return (cycle.StartMinuteOfWeek <= EndMinuteOfWeek || cycle.EndMinuteOfWeek >= StartMinuteOfWeek);
            }
            else
            {
                return true;
            }
           
        }

        public bool IsRunning(DateTime dateTime)
        {
            bool isRunning = false;
            var minuteOfWeek = ToMinuteOfWeek(dateTime.DayOfWeek, dateTime.Hour, dateTime.Minute);
            if (EndMinuteOfWeek > StartMinuteOfWeek)
                isRunning = (StartMinuteOfWeek <= minuteOfWeek && EndMinuteOfWeek >= minuteOfWeek);
            else
                isRunning = ((minuteOfWeek >= StartMinuteOfWeek) || (minuteOfWeek <= EndMinuteOfWeek));
            return isRunning;
        }

        protected int ToMinuteOfWeek(DayOfWeek dayOfWeek, int startHour, int startMinute)
        {
            var dayOfWeekMinutes = (int)dayOfWeek * 24 * 60;
            var startHourMinutes = startHour * 60;
            return dayOfWeekMinutes + startHourMinutes + startMinute;
        }
    }
}
