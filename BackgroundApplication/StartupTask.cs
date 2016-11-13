using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using SprinklerCore;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace BackgroundApplication
{
    public sealed class StartupTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // 
            // TODO: Insert code to perform background work
            //
            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral
            //
            var sprinklerController = new SprinklerController();
            var zoneConfig = new ZoneConfig { ZoneNumber = 1, Time = 10 };
            sprinklerController.AddWateringCycle(
        new CycleConfig()
        {
            DayOfWeek = SprinklerCore.DayOfWeek.Monday,
            StartHour = 9,
            StartMinute = 30,
            ZoneConfigs = new ZoneConfig[] { zoneConfig }
        });
        }
    }
}
