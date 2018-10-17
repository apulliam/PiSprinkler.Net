using System;
using Windows.System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.Storage;
using Newtonsoft.Json;
using SprinklerDotNet;
using System.Threading.Tasks;
using SprinklerDotNet.Config;

namespace PiSprinkler
{
    internal sealed class Sprinkler : SprinklerBase
    {
        private bool _done = false;
        private ThreadPoolTimer _timer = null;
        
        protected override async Task<IEnumerable<ZoneBase>> ReadZoneConfig()
        {
            var packageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var zoneFile = await packageFolder.GetFileAsync("ZoneConfig.json");
            String serializedZones = await FileIO.ReadTextAsync(zoneFile);
            return JsonConvert.DeserializeObject<List<Zone>>(serializedZones).Cast<ZoneBase>();
        }

        protected override async Task<IEnumerable<CycleProgram>> ReadCyclePrograms()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var programs = await localFolder.TryGetItemAsync("Programs.json");
            if (programs != null)
            {
                var serializedPrograms = await FileIO.ReadTextAsync(programs as StorageFile);
                if (serializedPrograms != null)
                {
                    return JsonConvert.DeserializeObject<List<CycleProgram>>(serializedPrograms);
                }
            }
            return new List<CycleProgram>();
        }

        protected override async Task WriteCyclePrograms(IEnumerable<CycleProgram> cyclePrograms)
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var programFile = await localFolder.CreateFileAsync("Programs.json", CreationCollisionOption.ReplaceExisting);
            var serializedPrograms = JsonConvert.SerializeObject(cyclePrograms);
            await FileIO.WriteTextAsync(programFile as StorageFile, serializedPrograms);
        }

        public override Task StartScheduler()
        {
           _timer = ThreadPoolTimer.CreateTimer(SchedulerTick, TimeSpan.FromSeconds(1));
            return Task.CompletedTask;
        }

        protected override void SchedulerTick(object state)
        {
            if (!_done)
            {
                base.SchedulerTick(state);
                if (!_done)
                {
                    _timer = ThreadPoolTimer.CreateTimer(SchedulerTick, TimeSpan.FromSeconds(1));
                }
            }
        }

        public override Task StopScheduler()
        {
            _done = true;
            if (_timer != null)
                _timer.Cancel();
            return Task.CompletedTask;
        }

    }
}
