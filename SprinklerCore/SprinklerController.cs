﻿using System;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace SprinklerCore
{
    public class SprinklerController 
    {
        private int MaxTime = 60;
        private List<Program> _programs = new List<Program>();
        private List<WateringCycle> _cycles = new List<WateringCycle>();
        private Dictionary<int, ZoneController> _zoneControllers = new Dictionary<int, ZoneController>();
        private ZoneController _manualZone = null;
        private bool _done = false;
        private Timer _timer = null;

        private async Task ReadZoneConfig()
        {
            // Server.MapPath("~/data.txt")
            String serializedZones = await File.ReadAllTextAsync("ZoneConfig.json");
            _zoneControllers = JsonConvert.DeserializeObject<Dictionary<int,ZoneController>>(serializedZones);
        }

        private async Task ReadPrograms()
        {
            var serializedPrograms = await File.ReadAllTextAsync("Programs.json");
            if (serializedPrograms != null)
            {
                _programs = JsonConvert.DeserializeObject<List<Program>>(serializedPrograms);
                foreach(var program in _programs)
                    _cycles.AddRange(WateringCycle.ToWateringCycles(program));
            }
            
        }

        private void WritePrograms()
        {
            File.WriteAllText("Programs.json", JsonConvert.SerializeObject(_programs));
        }

        public SprinklerController()
        {
            ReadZoneConfig().Wait();
            ReadPrograms().Wait();
        }

        private void ValidateZone(int zone)
        {
            if (!_zoneControllers.ContainsKey(zone))
            {
                var validZones = _zoneControllers.Keys.ToArray().ToString();
                throw new SprinklerControllerException("Zone must be " + validZones);
            }
        }

        private void ValidateTime(int time)
        {
            if (time <= 0 || time > MaxTime)
                throw new SprinklerControllerException("Zone time must be 1-" + MaxTime);
        }

        private void ValidateCycleConfigs(IEnumerable<CycleConfig> cycleConfigs)
        {
            foreach (var cycleConfig in cycleConfigs)
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
                    ValidateZone(zoneTime.ZoneNumber);
                    ValidateTime(zoneTime.RunTime);
                }
            }
        }

        public IEnumerable<ZoneController> GetAllZones()
        {
            return _zoneControllers.Values;
        }

        public ZoneController GetZone(int zoneNumber)
        {
            _zoneControllers.TryGetValue(zoneNumber, out ZoneController zoneController);
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

        public async Task<Guid> AddProgram(Program programConfig)
        {

            ValidateCycleConfigs(programConfig.Cycles);
            
            lock (this)
            {
                var newCycles = WateringCycle.ToWateringCycles(programConfig);
                foreach (var newCycle in newCycles)
                    foreach (var cycle in _cycles)
                        if (newCycle.ConflictsWith(cycle))
                            throw new SprinklerControllerException($"Program overlaps with program {cycle.Parent.Id}, cycle {cycle.Id}");
                _cycles.AddRange(newCycles);
                
                _programs.Add(programConfig);
                WritePrograms();
            }
            return programConfig.Id;
        }

        public async Task<bool> DeleteProgram(Guid programId)
        {
            var response = false;
            lock (this)
            {
                var programToDelete = _programs.Find(program => program.Id == programId);
                if (programToDelete != null)
                {
                    response = _programs.Remove(programToDelete);
                    _cycles.Where(cycle => cycle.Parent == programToDelete).Select(cycle => _cycles.Remove(cycle));
                    WritePrograms();
                }
            }
            return response;
        }

        public void ClearAllPrograms()
        {
            lock (this)
            {
                _programs.Clear();
                _cycles.Clear();
                WritePrograms();
            }
        }

        public Program GetProgram(Guid programId)
        {
            lock (this)
            {
                return _programs.Where(program => program.Id == programId).FirstOrDefault();
            }
        }

        public IEnumerable<WateringCycle> GetAllWateringCycles()
        {
            lock (this)
            {
                return _cycles;
            }
        }

        public IEnumerable<WateringCycle> GetWateringCyclesByZone(int zoneNumber)
        {
            ValidateZone(zoneNumber);
            lock (this)
            {
                return _cycles.Where(cycle => cycle.Zones.Any(zone => zone.Id == zoneNumber));
            }
        }

        public IEnumerable<Program> GetAllPrograms()
        {
            lock (this)
            {
                return _programs;
            }
        }

        public void RunScheduler()
        {
            // _timer = ThreadPoolTimer.CreateTimer(Scheduler, TimeSpan.FromSeconds(1));
            // trigger the timer immediately on startup
            _timer = new Timer(Scheduler, null, 0, Timeout.Infinite);
           
        }

        private void Scheduler(object state)
        {
            // Disable timer during processing
            _timer.Change(Timeout.Infinite, Timeout.Infinite);

            if (!_done)
            {
                var currentTime = DateTime.Now;

                lock (this)
                {
                    ZoneController turnOn = null;
                    IEnumerable<ZoneController> turnOff = null;
                    if (_manualZone == null)
                    {
                        var runningCycle = _cycles.Where(cycle => cycle.IsRunning(currentTime.DayOfWeek, currentTime.Hour, currentTime.Minute)).FirstOrDefault();
                        if (runningCycle != null)
                        {
                            Debug.WriteLine("Program {0}({1}), cycle {2} is running", runningCycle.Parent.Name, runningCycle.Parent.Id, runningCycle.Id);
                            var runningZone = runningCycle.Zones.Where(zone => zone.IsRunning(currentTime.DayOfWeek, currentTime.Hour, currentTime.Minute)).FirstOrDefault();
                            if (runningZone != null)
                            {
                                Debug.WriteLine("Current time is " + currentTime.TimeOfDay + " zone " + runningZone.Id + " is running.");
                                turnOn = _zoneControllers[runningZone.Id];
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
                    // Convert from ThreadPoolTimer.CreateTimer
                    _timer.Change(1000, Timeout.Infinite);
                }
            }
        }

        public void StopWateringCycles()
        {
            _done = true;
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

    }
}
