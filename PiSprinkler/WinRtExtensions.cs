using System.Collections.Generic;

namespace PiSprinkler
{
    internal static class WinRtExtensions
    {
        public static System.DayOfWeek ToInternal(this DayOfWeek dayOfWeek)
        {
            int intValue = (int)dayOfWeek;
            return (System.DayOfWeek)intValue;
        }

        public static SprinklerCore.CycleConfig ToInternal(this CycleConfig cycleConfig)
        {
            var internalCycleConfig = new SprinklerCore.CycleConfig()
            {
                DayOfWeek = cycleConfig.DayOfWeek.ToInternal(),
                StartHour = cycleConfig.StartHour,
                StartMinute = cycleConfig.StartMinute
            };
            var internalZoneConfigs = new List<SprinklerCore.ZoneConfig>();
            foreach (var zoneConfig in cycleConfig.ZoneConfigs)
                internalZoneConfigs.Add(zoneConfig.ToInternal());
            internalCycleConfig.ZoneConfigs = internalZoneConfigs;
            return internalCycleConfig;

        }
        public static SprinklerCore.ZoneConfig ToInternal(this ZoneConfig zoneConfig)
        {
            return new SprinklerCore.ZoneConfig()
            {
                ZoneNumber = zoneConfig.ZoneNumber,
                Time = zoneConfig.Time
            };
        }
    }
}
