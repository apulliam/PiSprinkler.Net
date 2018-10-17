using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SprinklerDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PiSprinkler.AspNet.Services
{
   
    internal class SchedulerService : IHostedService
    {
        private readonly SprinklerBase _sprinkler;
 

        public SchedulerService(SprinklerBase sprinkler)
        {
            _sprinkler = sprinkler;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _sprinkler.Initialize();
            await _sprinkler.StartScheduler();
            
        }
        

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _sprinkler.StopScheduler();
        }

      
    }
}
