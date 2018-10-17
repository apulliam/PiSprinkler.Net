using Microsoft.VisualStudio.TestTools.UnitTesting;
using SprinklerDotNet.Config;
using System;
using System.Threading.Tasks;

namespace SprinklerDotNet.UnitTest
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public async Task TestNonOverlappingCycles()
        {
            var sprinklerController = new TestSprinklerController();
            var zoneConfig = new ZoneProgram { ZoneNumber = 1, RunTime = 10 };

            await sprinklerController.AddProgram(new CycleProgram()
            {
                DaysOfWeek = new DayOfWeek[] { DayOfWeek.Monday },
                StartHour = 9,
                StartMinute = 30,
                Zones = new ZoneProgram[] { zoneConfig }
            });

            await sprinklerController.AddProgram(new CycleProgram()
            {
                DaysOfWeek = new DayOfWeek[] { DayOfWeek.Monday },
                StartHour = 9,
                StartMinute = 41,
                Zones = new ZoneProgram[] { zoneConfig }
            });
        }

        [TestMethod]
        public async Task TestOverlappingCycles()
        {
            var sprinklerController = new TestSprinklerController();
            var zoneConfig = new ZoneProgram { ZoneNumber = 1, RunTime = 10 };

            await sprinklerController.AddProgram(new CycleProgram()
            {
                DaysOfWeek = new DayOfWeek[] { DayOfWeek.Monday },
                StartHour = 9,
                StartMinute = 30,
                Zones = new ZoneProgram[] { zoneConfig }

            });

            Assert.ThrowsException<SprinklerControllerException>(() => sprinklerController.AddProgram(
                new CycleProgram()
                {
                    DaysOfWeek = new DayOfWeek[] { DayOfWeek.Monday },
                    StartHour = 9,
                    StartMinute = 35,
                    Zones = new ZoneProgram[] { zoneConfig }
                }));

        }

        [TestMethod]
        public async Task TestOverlappingCyclesWeekRollover()
        {
            var sprinklerController = new TestSprinklerController();
            var zoneConfig = new ZoneProgram { ZoneNumber = 1, RunTime = 10 };


            await sprinklerController.AddProgram(new CycleProgram()
            {
                DaysOfWeek = new DayOfWeek[] { DayOfWeek.Saturday },
                StartHour = 23,
                StartMinute = 55,
                Zones = new ZoneProgram[] { zoneConfig }
            });

            Assert.ThrowsException<SprinklerControllerException>(() => sprinklerController.AddProgram(new CycleProgram()
            {

                DaysOfWeek = new DayOfWeek[] { DayOfWeek.Sunday },
                StartHour = 0,
                StartMinute = 0,
                Zones = new ZoneProgram[] { zoneConfig }

            }));
        }



        [TestMethod]
        public async Task TestRunningZone()
        {
            var currentTime = DateTime.Now;
            var startTime = currentTime.Subtract(TimeSpan.FromSeconds(90));

            var sprinklerController = new TestSprinklerController();
            var zoneConfigs = new ZoneProgram[]
            {
                new ZoneProgram { ZoneNumber = 1, RunTime = 1 },
                new ZoneProgram { ZoneNumber = 5, RunTime = 1 }
            };

            var programId = await sprinklerController.AddProgram(new CycleProgram()
            {
                DaysOfWeek = new DayOfWeek[] { startTime.DayOfWeek },
                StartHour = startTime.Hour,
                StartMinute = startTime.Minute,
                Zones = zoneConfigs
            });
            var zone1 = await sprinklerController.GetZone(1);
            var zone2 = await sprinklerController.GetZone(2);

            await System.Threading.Tasks.Task.Delay(3000);
            Assert.IsTrue(zone1.IsRunning);
            Assert.IsFalse(zone2.IsRunning);

            zone1 = await sprinklerController.GetZone(1);
            zone2 = await sprinklerController.GetZone(2);

            Assert.IsFalse(zone1.IsRunning);
            Assert.IsTrue(zone2.IsRunning);

        }
    }
}
