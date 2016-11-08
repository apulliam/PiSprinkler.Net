using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SprinklerCore
{
    public class SprinklerController //: ISprinklerController
    {
        private int MaxTime = 60;
        private List<WateringCycle> _cycles = new List<WateringCycle>();
        private Dictionary<int, ZoneController> _zoneControllers = new Dictionary<int, ZoneController>();
        private bool _done = false;

        private Dictionary<int, int> _zoneGpioMap = new Dictionary<int, int>()
        {
            { 1, 18 },
            { 2, 23 },
            { 3, 24 },
            { 4, 25 },
            { 5, 12 },
            { 6, 16 },
            { 7, 20 },
            { 8, 21 }
        };

        private void InitializeZones()
        {
            foreach (var zone in _zoneGpioMap)
            {
                _zoneControllers[zone.Key] = new ZoneController(zone.Value);
            }
        }

        public SprinklerController()
        {
            InitializeZones();
        }

        private void ValidateZone(int zone)
        {
            if (!_zoneGpioMap.ContainsKey(zone))
            {
                var validZones = _zoneControllers.Keys.ToArray().ToString();
                throw new SprinklerControllerException("Zone must be " + validZones);
            }
        }

        private void ValidateZoneTimes(int[][] zoneTimes)
        {
            for (int i = 0; i < zoneTimes.Length; i++)
            {
                ValidateZone(zoneTimes[i][0]);
                ValidateTime(zoneTimes[i][1]);
            }
        }

        private void ValidateTime(int time)
        {
            if (time <= 0 || time > MaxTime)
                throw new SprinklerControllerException("Zone time must be 1-" + MaxTime);
        }

        private void ValidateHour(int hour)
        {
            if (hour < 0 || hour > 23)
                throw new SprinklerControllerException("Hour must be 0-23");
        }

        private void ValidateMinute(int hour)
        {
            if (hour < 0 || hour > 59)
                throw new SprinklerControllerException("Minute must be 0-59");
        }

        public IEnumerable<int> Zones
        {
            get
            {
                return _zoneGpioMap.Keys;
            }
        }

        public bool IsZoneRunning(int zoneNumber)
        {
            ValidateZone(zoneNumber);
            var zoneController = _zoneControllers[zoneNumber];
            return zoneController.IsRunning();
        }


        public void StartZone(int zoneNumber)
        {
            ValidateZone(zoneNumber);
            var zoneController = _zoneControllers[zoneNumber];
            zoneController.IsManual = true;
            zoneController.Start();
        }

        public void StopZone(int zoneNumber)
        {
            ValidateZone(zoneNumber);
            var zoneController = _zoneControllers[zoneNumber];
            zoneController.IsManual = false;
            zoneController.Stop();
        }

        public Guid AddWateringCycle(DayOfWeek dayOfWeek, int startHour, int startMinute, int[][] zoneTimes)
        {
            ValidateHour(startHour);
            ValidateMinute(startMinute);
            ValidateZoneTimes(zoneTimes);
            
            var newCycle = new WateringCycle(dayOfWeek, startHour, startMinute, zoneTimes);

            foreach (var cycle in _cycles)
            {
                if (newCycle.ConflictsWith(cycle))
                    throw new SprinklerControllerException("Cycle overlaps with " + cycle.CycleId);
            }
            _cycles.Add(newCycle);
            return newCycle.CycleId;
        }


        public void DeleteWateringCycle(Guid cycleId)
        {
            var cycleToDelete = _cycles.Find(cycle => cycle.CycleId == cycleId);
            if (cycleToDelete != null)
                _cycles.Remove(cycleToDelete);
        }

        public WateringCycle GetWateringCycle(Guid cycleId)
        {
            return _cycles.Where(cycle => cycle.CycleId == cycleId).FirstOrDefault();
        }

        public IEnumerable<WateringCycle> GetWateringCyclesByZone(int zoneNumber)
        {
            ValidateZone(zoneNumber);
            return _cycles.Where(cycle => cycle.Zones.Any(zone => zone.ZoneId == zoneNumber));
        }
        
        public IEnumerable<WateringCycle> GetAllWateringCycles()
        {
            return _cycles;
        }

        public void RunWateringCycles()
        {
            while (!_done)
            {
                var currentTime = DateTime.Now;
               
                foreach (var cycle in _cycles)
                {
                    if (cycle.IsRunning(currentTime))
                    {
                        Debug.WriteLine("cycle " + cycle.CycleId + " is running");
                        foreach (var zone in cycle.Zones)
                        {
                            if (zone.IsRunning(currentTime))
                            {
                                Debug.WriteLine("cycle " + cycle.CycleId + " is running");
                                _zoneControllers[zone.ZoneId].Start();
                            }
                            else
                            {
                                _zoneControllers[zone.ZoneId].Stop();
                            }
                        }
                    }
                    else
                    {
                        foreach (var zone in cycle.Zones)
                        {
                            _zoneControllers[zone.ZoneId].Stop();
                        }
                    }
                }
            }
        }
        public void StopWateringCycles()
        {
            _done = true;
        }

    }
}
