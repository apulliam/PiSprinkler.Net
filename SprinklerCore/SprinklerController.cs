using System;
using Windows.System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace SprinklerCore
{
    public class SprinklerController 
    {
        private int MaxTime = 60;
        private List<WateringCycle> _cycles = new List<WateringCycle>();
        private Dictionary<int, ZoneController> _zoneControllers = new Dictionary<int, ZoneController>();
        private ZoneController _manualZone = null;
        private bool _done = false;
        private ThreadPoolTimer _timer = null;

        private async void ReadZoneConfig()
        {
            var packageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var zoneFile = await packageFolder.GetFileAsync("ZoneConfig.json");
            String serializedZones = await FileIO.ReadTextAsync(zoneFile);
            _zoneControllers = JsonConvert.DeserializeObject<Dictionary<int,ZoneController>>(serializedZones);
        }

        private async void ReadWateringCycles()
        {
            var localFolder =  ApplicationData.Current.LocalFolder;
            var cycleFile = await localFolder.TryGetItemAsync("WateringCycles.json");
            if (cycleFile != null)
            {
                var serializedCycles = await FileIO.ReadTextAsync(cycleFile as StorageFile);
                if (serializedCycles != null)
                    _cycles = JsonConvert.DeserializeObject<List<WateringCycle>>(serializedCycles);
            }
        }

        private async void WriteWateringCycles()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var cycleFile = await localFolder.CreateFileAsync("WateringCycles.json",CreationCollisionOption.ReplaceExisting);
            var serializedCycles = JsonConvert.SerializeObject(_cycles);
            await FileIO.WriteTextAsync(cycleFile as StorageFile, serializedCycles);
        }

        public SprinklerController()
        {
            ReadZoneConfig();
            ReadWateringCycles();
        }

        private void ValidateZone(int zone)
        {
            if (!_zoneControllers.ContainsKey(zone))
            {
                var validZones = _zoneControllers.Keys.ToArray().ToString();
                throw new SprinklerControllerException("Zone must be " + validZones);
            }
        }

        private void ValidateZoneTimes(IEnumerable<ZoneConfig> zoneTimes)
        {
            var distinct = zoneTimes.Distinct();
            if (distinct.Count() != zoneTimes.Count())
                throw new SprinklerControllerException("A zone can only be run once in each cylce");
            foreach (var zoneTime in zoneTimes)
            {
                ValidateZone(zoneTime.ZoneNumber);
                ValidateTime(zoneTime.Time);
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

        public IEnumerable<ZoneController> GetAllZones()
        {
            return _zoneControllers.Values;
        }

        public ZoneController GetZone(int zoneNumber)
        {
            ZoneController zoneController = null;
            _zoneControllers.TryGetValue(zoneNumber, out zoneController);
            return zoneController;
        }

        public bool IsZoneRunning(int zoneNumber)
        {
            ValidateZone(zoneNumber);
            var zoneController = _zoneControllers[zoneNumber];
            return _manualZone == zoneController || zoneController.IsRunning;
        }

        public void StartZone(int zoneNumber)
        {
            ValidateZone(zoneNumber);
            _manualZone = _zoneControllers[zoneNumber];
        }

        public void StopZone()
        {
            _manualZone = null;
        }

        public Guid AddWateringCycle(CycleConfig cycleConfig)
        {
            ValidateHour(cycleConfig.StartHour);
            ValidateMinute(cycleConfig.StartMinute);
            ValidateZoneTimes(cycleConfig.ZoneConfigs);
            
            var wateringCycle = new WateringCycle(cycleConfig);
            lock (_cycles)
            {
                foreach (var cycle in _cycles)
                {
                    if (wateringCycle.ConflictsWith(cycle))
                        throw new SprinklerControllerException("Cycle overlaps with " + cycle.CycleId);
                }
                _cycles.Add(wateringCycle);
                WriteWateringCycles();
            }
            return wateringCycle.CycleId;
        }

        public bool DeleteWateringCycle(Guid cycleId)
        {
            var response = false;
            lock (_cycles)
            {
                var cycleToDelete = _cycles.Find(cycle => cycle.CycleId == cycleId);
                if (cycleToDelete != null)
                {
                    response = _cycles.Remove(cycleToDelete);
                    WriteWateringCycles();
                }
            }
            return response;
        }

        public void ClearWateringCycles()
        {
            lock (_cycles)
            {
                _cycles.Clear();
                WriteWateringCycles();
            }
        }

        public WateringCycle GetWateringCycle(Guid cycleId)
        {
            lock (_cycles)
            {
                return _cycles.Where(cycle => cycle.CycleId == cycleId).FirstOrDefault();
            }
        }
        
        public IEnumerable<WateringCycle> GetWateringCyclesByZone(int zoneNumber)
        {
            ValidateZone(zoneNumber);
            lock (_cycles)
            {
                return _cycles.Where(cycle => cycle.Zones.Any(zone => zone.ZoneId == zoneNumber));
            }
        }
        
        public IEnumerable<WateringCycle> GetAllWateringCycles()
        {
            lock (_cycles)
            {
                return _cycles;
            }
        }

        public void RunWateringCycles()
        {
           _timer = ThreadPoolTimer.CreateTimer(Scheduler, TimeSpan.FromSeconds(1));
            
        }

        private void Scheduler(object state)
        {

            if (!_done)
            {
                var currentTime = DateTime.Now;

                lock (_cycles)
                {
                    ZoneController turnOn = null;
                    IEnumerable<ZoneController> turnOff = null;
                    if (_manualZone == null)
                    {
                        var runningCycle = _cycles.Where(cycle => cycle.IsRunning(currentTime)).FirstOrDefault();
                        if (runningCycle != null)
                        {
                            Debug.WriteLine("cycle " + runningCycle.CycleId + " is running");
                            var runningZone = runningCycle.Zones.Where(zone => zone.IsRunning(currentTime)).FirstOrDefault();
                            if (runningZone != null)
                            {
                                Debug.WriteLine("Current time is " + currentTime.TimeOfDay + " zone " + runningZone.ZoneId + " is running.");
                                turnOn = _zoneControllers[runningZone.ZoneId];
                            }

                        }

                    }
                    else
                    {
                        Debug.WriteLine("zone " + _manualZone.ZoneNumber + " is manual mode");

                        turnOn = _manualZone;

                    }

                    if (turnOn != null)
                        turnOff = _zoneControllers.Where(zoneController => zoneController.Value != turnOn).Select(zoneController => zoneController.Value);
                    else
                        turnOff = _zoneControllers.Select(zoneController => zoneController.Value);

                    turnOff.ToList().ForEach(zoneController => zoneController.Stop());
                    if (turnOn != null)
                        turnOn.Start();
                }
                if (!_done)
                {
                    _timer = ThreadPoolTimer.CreateTimer(Scheduler, TimeSpan.FromSeconds(1));
                }
                //await System.Threading.Tasks.Task.Delay(1000);
            }
        }

        public void StopWateringCycles()
        {
            _done = true;
            if (_timer != null)
                _timer.Cancel();
        }

    }
}
