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
            var zoneConfig = new ZoneConfig { ZoneNumber = 1, RunTime = 10 };

            sprinklerController.AddProgram(new SprinklerCore.Program("Program 1",
                new CycleConfig[]
                {
                    new CycleConfig()
                    {
                        DaysOfWeek = new DayOfWeek[] { DayOfWeek.Monday },
                        StartHour = 9,
                        StartMinute = 30,
                        Zones = new ZoneConfig[] { zoneConfig }
                    }
                }));

            sprinklerController.AddProgram(new SprinklerCore.Program("Program 2",
                
                new CycleConfig[]
                {
                    new CycleConfig()
                    {
                        DaysOfWeek = new DayOfWeek[] { DayOfWeek.Monday },
                        StartHour = 9,
                        StartMinute = 41,
                        Zones = new ZoneConfig[] { zoneConfig }
                    }
                
                }));
        }

        [TestMethod]
        public void TestOverlappingCycles()
        {
            var sprinklerController = new SprinklerController();
            var zoneConfig = new ZoneConfig { ZoneNumber = 1, RunTime = 10 };
            
            sprinklerController.AddProgram(new SprinklerCore.Program("Program 1",
                new CycleConfig[]
                {
                    new CycleConfig()
                    {
                        DaysOfWeek = new DayOfWeek[] { DayOfWeek.Monday },
                        StartHour = 9,
                        StartMinute = 30,
                        Zones = new ZoneConfig[] { zoneConfig }
                    }
                }));

            Assert.ThrowsException<SprinklerControllerException>(() => sprinklerController.AddProgram(new SprinklerCore.Program("Program 2",
                new CycleConfig[]
                {
                    new CycleConfig()
                    {
                        DaysOfWeek = new DayOfWeek[] { DayOfWeek.Monday },
                        StartHour = 9,
                        StartMinute = 35,
                        Zones = new ZoneConfig[] { zoneConfig }
                    }
                })));
        }

        [TestMethod]
        public void TestOverlappingCyclesWeekRollover()
        {
            var sprinklerController = new SprinklerController();
            var zoneConfig = new ZoneConfig { ZoneNumber = 1, RunTime = 10 };


            sprinklerController.AddProgram(new SprinklerCore.Program("Program 1", new CycleConfig[]
                {
                    new CycleConfig()
                    {
                        DaysOfWeek = new DayOfWeek[] { DayOfWeek.Saturday },
                        StartHour = 23,
                        StartMinute = 55,
                        Zones = new ZoneConfig[] { zoneConfig }
                    }
                }));

            Assert.ThrowsException<SprinklerControllerException>(() => sprinklerController.AddProgram(new SprinklerCore.Program("Program 2",
                new CycleConfig[]
                {
                    new CycleConfig()
                    {
                        DaysOfWeek = new DayOfWeek[] { DayOfWeek.Sunday },
                        StartHour = 0,
                        StartMinute = 0,
                        Zones = new ZoneConfig[] { zoneConfig }
                    }
                })));
        }



        [TestMethod]
        public void TestRunningZone()
        {
            var currentTime = DateTime.Now;
            var startTime = currentTime.Subtract(TimeSpan.FromSeconds(90));
          
            var sprinklerController = new SprinklerController();
            var zoneConfigs = new ZoneConfig[] 
            {
                new ZoneConfig { ZoneNumber = 1, RunTime = 1 },
                new ZoneConfig { ZoneNumber = 5, RunTime = 1 }
            };

            var programId = sprinklerController.AddProgram(new SprinklerCore.Program("Program 1", 
                new CycleConfig[]
                {
                    new CycleConfig()
                    {
                        DaysOfWeek = new DayOfWeek[] { startTime.DayOfWeek },
                        StartHour = startTime.Hour,
                        StartMinute = startTime.Minute,
                        Zones = zoneConfigs
                    }
                }));
            var zone1 = sprinklerController.GetZone(1);
            var zone2 = sprinklerController.GetZone(2);

            System.Threading.Tasks.Task.Delay(3000);
            Assert.IsTrue(zone1.IsRunning);
            Assert.IsFalse(zone2.IsRunning);

            zone1 = sprinklerController.GetZone(1);
            zone2 = sprinklerController.GetZone(2);

            Assert.IsFalse(zone1.IsRunning);
            Assert.IsTrue(zone2.IsRunning);

        }
    }
}
