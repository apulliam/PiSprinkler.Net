using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using SprinklerCore;

namespace SprinklerCoreUnitTest
{
    [TestClass]
    public class WateringCycleUnitTests
    {
        [TestMethod]
        public void TestNonOverlappingCycles()
        {
            var sprinklerController = new SprinklerController();
            var zoneConfig = new ZoneConfig { ZoneNumber= 1, Time= 10 };

            sprinklerController.AddWateringCycle(
                new CycleConfig()
                {
                    DayOfWeek = DayOfWeek.Monday,
                    StartHour = 9,
                    StartMinute = 30,
                    ZoneConfigs = new ZoneConfig[] { zoneConfig }
                });

            sprinklerController.AddWateringCycle(
                new CycleConfig()
                {
                    DayOfWeek = DayOfWeek.Monday,
                    StartHour = 9,
                    StartMinute = 41,
                    ZoneConfigs = new ZoneConfig[] { zoneConfig }
                });
        }

        [TestMethod]
        public void TestOverlappingCycles()
        {
            var sprinklerController = new SprinklerController();
            var zoneConfig = new ZoneConfig { ZoneNumber = 1, Time = 10 };
            
            sprinklerController.AddWateringCycle(
                new CycleConfig()
                {
                    DayOfWeek = DayOfWeek.Monday,
                    StartHour = 9,
                    StartMinute = 30,
                    ZoneConfigs = new ZoneConfig[] { zoneConfig }
                });

            Assert.ThrowsException<SprinklerControllerException>(() => sprinklerController.AddWateringCycle(
                new CycleConfig()
                {
                    DayOfWeek = DayOfWeek.Monday,
                    StartHour = 9,
                    StartMinute = 35,
                    ZoneConfigs = new ZoneConfig[] { zoneConfig }
                }));
        }

        [TestMethod]
        public void TestOverlappingCyclesWeekRollover()
        {
            var sprinklerController = new SprinklerController();
            var zoneConfig = new ZoneConfig { ZoneNumber = 1, Time = 10 };


            sprinklerController.AddWateringCycle(
                new CycleConfig()
                {
                    DayOfWeek = DayOfWeek.Saturday,
                    StartHour = 23,
                    StartMinute = 55,
                    ZoneConfigs = new ZoneConfig[] { zoneConfig }
                });

            Assert.ThrowsException<SprinklerControllerException>(() => sprinklerController.AddWateringCycle(
                new CycleConfig()
                {
                    DayOfWeek = DayOfWeek.Sunday,
                    StartHour = 0,
                    StartMinute = 0,
                    ZoneConfigs = new ZoneConfig[] { zoneConfig }
                }));
        }



        [TestMethod]
        public void TestRunningZone()
        {
            var currentTime = DateTime.Now;
            var startTime = currentTime.Subtract(TimeSpan.FromMinutes(5));
          
            var sprinklerController = new SprinklerController();
            var zoneConfigs = new ZoneConfig[] 
            {
                new ZoneConfig { ZoneNumber = 1, Time = 10 },
                new ZoneConfig { ZoneNumber = 5, Time = 10 }
            };

            var cycleId = sprinklerController.AddWateringCycle(
                new CycleConfig()
                {
                    DayOfWeek = startTime.DayOfWeek,
                    StartHour = startTime.Hour,
                    StartMinute = startTime.Minute,
                    ZoneConfigs = zoneConfigs
                });
            var cycle = sprinklerController.GetWateringCycle(cycleId);

            Assert.IsTrue(cycle.Zones[0].IsRunning(currentTime));
            Assert.IsFalse(cycle.Zones[1].IsRunning(currentTime));

            currentTime = currentTime.AddMinutes(10);

            Assert.IsFalse(cycle.Zones[0].IsRunning(currentTime));
            Assert.IsTrue(cycle.Zones[1].IsRunning(currentTime));

        }
    }
}
