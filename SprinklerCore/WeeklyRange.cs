using System;

namespace SprinklerCore
{
    public abstract class WeeklyRange
    {
        protected int StartMinuteOfWeek
        {
            get;
            set;
        }

        private int _endMinuteOfWeek;

        protected int EndMinuteOfWeek
        {
            get
            {
                return _endMinuteOfWeek;
            }
            set
            {
                if (value > 10080)
                {
                    _overlapMinuteOfWeek = value - 10080;
                }
                else
                {
                    _endMinuteOfWeek = value;
                }
            }
        }
        private int _overlapMinuteOfWeek;

        protected int OverlapMinuteOfWeek
        {
            get
            {
                return _overlapMinuteOfWeek;
            }
        }

        public bool IsRunning(DateTime dateTime)
        {
            var minuteOfWeek = WateringCycle.ToMinuteOfWeek(dateTime.DayOfWeek, dateTime.Hour, dateTime.Minute);
            return (StartMinuteOfWeek <= minuteOfWeek && EndMinuteOfWeek >= minuteOfWeek);
        }

    }
}
