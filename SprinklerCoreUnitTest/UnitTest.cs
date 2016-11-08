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
            int[][] zoneTimes = new int[][] { new int[] { 1, 10 } };

            sprinklerController.AddWateringCycle(SprinklerCore.DayOfWeek.Monday, 9, 30, zoneTimes);

            sprinklerController.AddWateringCycle(SprinklerCore.DayOfWeek.Monday, 9, 41, zoneTimes);
        }

        [TestMethod]
        public void TestOverlappingCycles()
        {
            var sprinklerController = new SprinklerController();
            int[][] zoneTimes = new int[][] { new int[] { 1, 10 } };

            sprinklerController.AddWateringCycle(SprinklerCore.DayOfWeek.Monday, 9, 30, zoneTimes);

            Assert.ThrowsException<SprinklerControllerException>(() => sprinklerController.AddWateringCycle(SprinklerCore.DayOfWeek.Monday, 9, 35, zoneTimes));
        }

        [TestMethod]
        public void TestOverlappingCyclesWeekRollover()
        {
            var sprinklerController = new SprinklerController();
            int[][] zoneTimes = new int[][] { new int[] { 1, 10 } };

            sprinklerController.AddWateringCycle(SprinklerCore.DayOfWeek.Saturday, 23, 55, zoneTimes);

            Assert.ThrowsException<SprinklerControllerException>(() => sprinklerController.AddWateringCycle(SprinklerCore.DayOfWeek.Sunday, 0, 0, zoneTimes));
        }



        [TestMethod]
        public void TestRunningZone()
        {
            var currentTime = DateTime.Now;
            var startTime = currentTime.Subtract(TimeSpan.FromMinutes(5));
          
            var sprinklerController = new SprinklerController();
            int[][] zoneTimes = new int[][] { new int[] { 1, 10 }, new int[] { 5, 10 } };

            var cycleId = sprinklerController.AddWateringCycle(startTime.DayOfWeek.ToExportable(), startTime.Hour, startTime.Minute, zoneTimes);
            var cycle = sprinklerController.GetWateringCycle(cycleId);

            Assert.IsTrue(cycle.Zones[0].IsRunning(currentTime));
            Assert.IsFalse(cycle.Zones[1].IsRunning(currentTime));

            currentTime = currentTime.AddMinutes(10);

            Assert.IsFalse(cycle.Zones[0].IsRunning(currentTime));
            Assert.IsTrue(cycle.Zones[1].IsRunning(currentTime));

        }
    }
}
