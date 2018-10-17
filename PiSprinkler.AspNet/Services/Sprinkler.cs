using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SprinklerDotNet;
using System.Threading.Tasks;
using Microsoft.DotNet.PlatformAbstractions;
using System.IO;
using SprinklerDotNet.Config;

namespace PiSprinkler.AspNet.Services
{
    public class Sprinkler : SprinklerBase
    {
        private bool _done = false;
        private Timer _timer = null;

        public Sprinkler() 
        {
           
        }
        protected override Task<IEnumerable<ZoneBase>> ReadZoneConfig()
        {
            var packageFolder = ApplicationEnvironment.ApplicationBasePath;
            var zoneFile = $"{packageFolder}ZoneConfig.json";
            String serializedZones = File.ReadAllText(zoneFile);
            var zoneControllers = JsonConvert.DeserializeObject<List<Zone>>(serializedZones);
            return Task.FromResult(zoneControllers.Cast<ZoneBase>());
        }

        protected override Task<IEnumerable<CycleProgram>> ReadCyclePrograms()
        {
            var localFolder = ApplicationEnvironment.ApplicationBasePath;
            var configFile = $"{localFolder}Programs.json";
            if (File.Exists(configFile))
            {
                var serializedPrograms = File.ReadAllText(configFile);
                if (serializedPrograms != null)
                {
                    var programs = JsonConvert.DeserializeObject<List<CycleProgram>>(serializedPrograms, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                    return Task.FromResult(programs.AsEnumerable());

                }
            }
            return Task.FromResult(new List<CycleProgram>().AsEnumerable());
        }

        protected override Task WriteCyclePrograms(IEnumerable<CycleProgram> cycleConfigs)
        {
            var localFolder = ApplicationEnvironment.ApplicationBasePath;
            var programFile = $"{localFolder}Programs.json";
            var serializedPrograms = JsonConvert.SerializeObject(cycleConfigs, new JsonSerializerSettings() {  NullValueHandling = NullValueHandling.Ignore});
            File.WriteAllText(programFile, serializedPrograms);
            return Task.CompletedTask;
        }


        public override Task StartScheduler()
        {
            _timer = new Timer(SchedulerTick, this, 0, TimeSpan.FromSeconds(1).Milliseconds);
            return Task.CompletedTask;
        }

        protected override void SchedulerTick(object state)
        {
            _timer.Change(-1, 0);
            if (!_done)
            {
                base.SchedulerTick(state);
                if (!_done)
                {
                    _timer.Change(0,TimeSpan.FromSeconds(1).Milliseconds);
                }
            }
        }

        public override Task StopScheduler()
        {
            _done = true;
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
            return Task.CompletedTask;
        }

    }
}
