namespace SprinklerCore
{
    public static class WinRtExtensions
    {
        public static DayOfWeek ToExportable(this System.DayOfWeek dayOfWeek)
        {
            int intValue = (int)dayOfWeek;
            return (DayOfWeek)intValue;
        }
    }
}
