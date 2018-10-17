using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SprinklerDotNet.Config;
using SprinklerDotNet.Runtime;

namespace SprinklerDotNet
{
    public abstract class SprinklerBase 
    {
        private bool _initialized = false;
        private int MaxTime = 60;
        private List<CycleProgram> _cyclePrograms = null;
        private List<WateringCycle> _cycles = new List<WateringCycle>();
        private IEnumerable<ZoneBase> _zones = null;
        private ZoneBase _manualZone = null;
      
        protected abstract Task<IEnumerable<ZoneBase>> ReadZoneConfig();

        protected abstract Task<IEnumerable<CycleProgram>> ReadCyclePrograms();

        protected abstract Task WriteCyclePrograms(IEnumerable<CycleProgram> programs);

        public async Task Initialize()
        {
            if (!_initialized)
            {
                _zones = await ReadZoneConfig();
                _cyclePrograms = (await ReadCyclePrograms()).ToList();
                foreach (CycleProgram cycle in _cyclePrograms)
                    _cycles.AddRange(cycle.ToWateringCycles());
                _initialized = true;
            }
        }

        private async Task<ZoneBase> ValidateZone(int zoneNumber)
        {
            await Initialize();
            var zone = await GetZone(zoneNumber);
            if (zone == null)
            {
                var validZones = _zones.Select(z => z.ZoneNumber).ToArray().ToString();
                throw new SprinklerControllerException("Zone must be " + validZones);
            }
            return zone;
        }

        private void ValidateTime(int time)
        {
            if (time <= 0 || time > MaxTime)
                throw new SprinklerControllerException("Zone time must be 1-" + MaxTime);
        }

        private async Task ValidateCycleConfig(CycleProgram cycleConfig)
        {
            if (cycleConfig.StartHour < 0 || cycleConfig.StartHour > 23)
                throw new SprinklerControllerException("Hour must be 0-23");

            if (cycleConfig.StartMinute < 0 || cycleConfig.StartMinute > 59)
                throw new SprinklerControllerException("Minute must be 0-59");

            var distinct = cycleConfig.Zones.Distinct();
            if (distinct.Count() != cycleConfig.Zones.Count())
                throw new SprinklerControllerException("A zone can only be run once in each cylce");

            foreach (var zoneTime in cycleConfig.Zones)
            {
                await ValidateZone(zoneTime.ZoneNumber);
                ValidateTime(zoneTime.RunTime);
            }
        }

        public async Task<IEnumerable<ZoneBase>> GetZones()
        {
            await Initialize();
            return _zones;
        }

        public async Task<ZoneBase> GetZone(int zoneNumber)
        {
            await Initialize();
            return _zones.Where(zone => zone.ZoneNumber == zoneNumber).FirstOrDefault();
        }

        public async Task<bool> IsZoneRunning(int zoneNumber)
        {
            var zoneController = await GetZone(zoneNumber);
            return _manualZone == zoneController || zoneController.IsRunning;
        }

        public async Task StartZone(int zoneNumber)
        {
            _manualZone = await GetZone(zoneNumber);
        }

        public Task StopZone()
        {
            _manualZone = null;
            return Task.CompletedTask;
        }

        public async Task<Guid> AddProgram(CycleProgram cycleConfig)
        {
            await Initialize();
            await ValidateCycleConfig(cycleConfig);
            var newCycles = cycleConfig.ToWateringCycles();
            lock (this)
            {
                foreach (var newCycle in newCycles)
                    foreach (var wateringCycle in _cycles)
                        if (newCycle.ConflictsWith(wateringCycle))
                            throw new SprinklerControllerException($"Program overlaps with program {wateringCycle.Program.Id}, cycle {wateringCycle.Id}");
                _cycles.AddRange(newCycles);

                _cyclePrograms.Add(cycleConfig);
            }
            await WriteCyclePrograms(_cyclePrograms);
            
            return cycleConfig.Id;
        }

        public async Task<CycleProgram> GetProgram(Guid cycleId)
        {
            await Initialize();
            lock (this)
            {
                return _cyclePrograms.Where(cycle => cycle.Id == cycleId).FirstOrDefault();
            }
        }
        
        public async Task<IEnumerable<CycleProgram>> GetPrograms()
        {
            await Initialize();
            lock (this)
            {
                return _cyclePrograms;
            }
        }

        public async Task<bool> DeleteProgram(Guid programId)
        {
            await Initialize();
            var response = false;
            lock (this)
            {
                var programToDelete = _cyclePrograms.Find(program => program.Id == programId);
                if (programToDelete != null)
                {
                    response = _cyclePrograms.Remove(programToDelete);
                    _cycles.Where(cycle => cycle.Program == programToDelete).Select(cycle => _cycles.Remove(cycle));

                }
            }
            await WriteCyclePrograms(_cyclePrograms);
            return response;
        }

        public async Task ClearPrograms()
        {
            await Initialize();
            lock (this)
            {
                _cyclePrograms.Clear();
                _cycles.Clear();
            }
            await WriteCyclePrograms(_cyclePrograms);
        }

        public async Task<IEnumerable<WateringCycle>> GetWateringCycles()
        {
            await Initialize();
            lock (this)
            {
                return _cycles;
            }
        }

        public async Task<IEnumerable<WateringCycle>> GetWateringCyclesByZone(int zoneNumber)
        {
            await Initialize();
            await GetZone(zoneNumber);
            lock (this)
            {
                return _cycles.Where(cycle => cycle.Zones.Any(zone => zone.ZoneNumber == zoneNumber));
            }
        }

        public abstract Task StartScheduler();

        public abstract Task StopScheduler();

        protected virtual void SchedulerTick(object state)
        {
            var currentTime = DateTime.Now;

            lock (this)
            {
                ZoneBase turnOn = null;
                IEnumerable<ZoneBase> turnOff = null;
                if (_manualZone == null)
                {
                    var runningCycle = _cycles.Where(cycle => cycle.IsRunning(currentTime.DayOfWeek, currentTime.Hour, currentTime.Minute)).FirstOrDefault();
                    if (runningCycle != null)
                    {
                        Debug.WriteLine("Program {0}({1}), cycle {2} is running", runningCycle.Program.Name, runningCycle.Program.Id, runningCycle.Id);
                        var runningZone = runningCycle.Zones.Where(zone => zone.IsRunning(currentTime.DayOfWeek, currentTime.Hour, currentTime.Minute)).FirstOrDefault();
                        if (runningZone != null)
                        {
                            Debug.WriteLine("Current time is " + currentTime.TimeOfDay + " zone " + runningZone.ZoneNumber + " is running.");
                            turnOn = _zones.Where(zone=> zone.ZoneNumber == runningZone.ZoneNumber).FirstOrDefault();
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("zone " + _manualZone.ZoneNumber + " is manual mode");
                    turnOn = _manualZone;
                }

                if (turnOn != null)
                    turnOff = _zones.Where(zoneController => zoneController != turnOn);
                else
                    turnOff = _zones;

                turnOff.ToList().ForEach(zoneController => zoneController.Stop());
                if (turnOn != null)
                    turnOn.Start();
            }
        }
    }
}
