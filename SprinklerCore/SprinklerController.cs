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
        private bool _done = false;
        private ThreadPoolTimer _timer = null;

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

        private async Task ReadZoneGpioMap()
        {
            var packageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var mapFile = await packageFolder.GetFileAsync("ZoneGpioMap.json");
            String serializedMap = await FileIO.ReadTextAsync(mapFile);
            _zoneGpioMap = JsonConvert.DeserializeObject<Dictionary<int, int>>(serializedMap);
        }

        private async Task ReadWateringCycles()
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

        private async Task WriteWateringCycles()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var cycleFile = await localFolder.CreateFileAsync("WateringCycles.json",CreationCollisionOption.ReplaceExisting);
            var serializedCycles = JsonConvert.SerializeObject(_cycles);
            await FileIO.WriteTextAsync(cycleFile as StorageFile, serializedCycles);

        }

        private async void InitializeZones()
        {
            await ReadZoneGpioMap();
            //var serializedZoneMap = JsonConvert.SerializeObject(_zoneGpioMap);
            foreach (var zone in _zoneGpioMap)
            {
                _zoneControllers[zone.Key] = new MockZoneController(zone.Value);
            };
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

        private void ValidateZoneTimes(IEnumerable<ZoneConfig> zoneTimes)
        {
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
                    response = _cycles.Remove(cycleToDelete);
            }
            return response;
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
           _timer = Windows.System.Threading.ThreadPoolTimer.CreateTimer(TimerCallback, TimeSpan.FromSeconds(1));
            
        }

        private void TimerCallback(object state)
        {

            if (!_done)
            {
                var currentTime = DateTime.Now;

                lock (_cycles)
                {
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

                if (!_done)
                {
                    _timer = ThreadPoolTimer.CreateTimer(TimerCallback, TimeSpan.FromSeconds(1));
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
