using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace SprinklerCore
{
   

   

    public interface ISprinklerController
    {
        Guid AddWateringCycle(int zone, DayOfWeek dayOfWeek, int startHour, int startMinute, int runMinutes);
        void DeleteWateringCycle(Guid cycleId);
        Guid RunZone(int zone, int runMinutes);
        bool StopZone(int zone);
        IEnumerable<WateringCycle> GetWateringCyclesByZone(int zone);
        IEnumerable<WateringCycle> GetAllWateringCycles();
    }

}
