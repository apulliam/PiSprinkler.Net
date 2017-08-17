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

            sprinklerController.AddProgram(new ProgramConfig("Program 1",
                new CycleConfig[]
                {
                    new CycleConfig()
                    {
                        DaysOfWeek = new DayOfWeek[] { DayOfWeek.Monday },
                        StartHour = 9,
                        StartMinute = 30,
                        ZoneConfigs = new ZoneConfig[] { zoneConfig }
                    }
                }));

            sprinklerController.AddProgram(new ProgramConfig("Program 2",
                
                new CycleConfig[]
                {
                    new CycleConfig()
                    {
                        DaysOfWeek = new DayOfWeek[] { DayOfWeek.Monday },
                        StartHour = 9,
                        StartMinute = 41,
                        ZoneConfigs = new ZoneConfig[] { zoneConfig }
                    }
                
                }));
        }

        [TestMethod]
        public void TestOverlappingCycles()
        {
            var sprinklerController = new SprinklerController();
            var zoneConfig = new ZoneConfig { ZoneNumber = 1, RunTime = 10 };
            
            sprinklerController.AddProgram(new ProgramConfig("Program 1",
                new CycleConfig[]
                {
                    new CycleConfig()
                    {
                        DaysOfWeek = new DayOfWeek[] { DayOfWeek.Monday },
                        StartHour = 9,
                        StartMinute = 30,
                        ZoneConfigs = new ZoneConfig[] { zoneConfig }
                    }
                }));

            Assert.ThrowsException<SprinklerControllerException>(() => sprinklerController.AddProgram(new ProgramConfig("Program 2",
                new CycleConfig[]
                {
                    new CycleConfig()
                    {
                        DaysOfWeek = new DayOfWeek[] { DayOfWeek.Monday },
                        StartHour = 9,
                        StartMinute = 35,
                        ZoneConfigs = new ZoneConfig[] { zoneConfig }
                    }
                })));
        }

        [TestMethod]
        public void TestOverlappingCyclesWeekRollover()
        {
            var sprinklerController = new SprinklerController();
            var zoneConfig = new ZoneConfig { ZoneNumber = 1, RunTime = 10 };


            sprinklerController.AddProgram(new ProgramConfig("Program 1", new CycleConfig[]
                {
                    new CycleConfig()
                    {
                        DaysOfWeek = new DayOfWeek[] { DayOfWeek.Saturday },
                        StartHour = 23,
                        StartMinute = 55,
                        ZoneConfigs = new ZoneConfig[] { zoneConfig }
                    }
                }));

            Assert.ThrowsException<SprinklerControllerException>(() => sprinklerController.AddProgram(new ProgramConfig("Program 2",
                new CycleConfig[]
                {
                    new CycleConfig()
                    {
                        DaysOfWeek = new DayOfWeek[] { DayOfWeek.Sunday },
                        StartHour = 0,
                        StartMinute = 0,
                        ZoneConfigs = new ZoneConfig[] { zoneConfig }
                    }
                })));
        }



        [TestMethod]
        public void TestRunningZone()
        {
            var currentTime = DateTime.Now;
            var startTime = currentTime.Subtract(TimeSpan.FromMinutes(5));
          
            var sprinklerController = new SprinklerController();
            var zoneConfigs = new ZoneConfig[] 
            {
                new ZoneConfig { ZoneNumber = 1, RunTime = 10 },
                new ZoneConfig { ZoneNumber = 5, RunTime = 10 }
            };

            var programId = sprinklerController.AddProgram(new ProgramConfig("Program 1", 
                new CycleConfig[]
                {
                    new CycleConfig()
                    {
                        DaysOfWeek = new DayOfWeek[] { startTime.DayOfWeek },
                        StartHour = startTime.Hour,
                        StartMinute = startTime.Minute,
                        ZoneConfigs = zoneConfigs
                    }
                }));
            var program = sprinklerController.GetProgram(programId);

            Assert.IsTrue(program.Cycles[0].Zones[0].IsRunning(currentTime.DayOfWeek, currentTime.Hour, currentTime.Minute));
            Assert.IsFalse(program.Cycles[0].Zones[1].IsRunning(currentTime.DayOfWeek, currentTime.Hour, currentTime.Minute));

            currentTime = currentTime.AddMinutes(10);

            Assert.IsFalse(program.Cycles[0].Zones[0].IsRunning(currentTime.DayOfWeek, currentTime.Hour, currentTime.Minute));
            Assert.IsTrue(program.Cycles[0].Zones[1].IsRunning(currentTime.DayOfWeek, currentTime.Hour, currentTime.Minute));

        }
    }
}
