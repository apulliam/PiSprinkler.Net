using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SprinklerCore;

namespace PiSprinkler.Services
{
    #region snippet1
    public class SprinklerService : IHostedService//, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<SprinklerService> _logger;
        private Timer _timer;
        private SprinklerController _sprinklerController;

        public SprinklerService(SprinklerController sprinklerController, ILogger<SprinklerService> logger)
        {
            _logger = logger;
            _sprinklerController = sprinklerController;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _sprinklerController.RunScheduler();

            //_timer = new Timer(DoWork, null, TimeSpan.Zero,
            //    TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        //private void DoWork(object state)
        //{
        //    executionCount++;

        //    _logger.LogInformation(
        //        "Timed Hosted Service is working. Count: {Count}", executionCount);
        //}

        public Task StopAsync(CancellationToken stoppingToken)
        {
            //_logger.LogInformation("Timed Hosted Service is stopping.");

            //_timer?.Change(Timeout.Infinite, 0);
            _sprinklerController.StopWateringCycles();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            //_timer?.Dispose();
            //_sprinklerController?.Dispose();
        }
    }
    #endregion
}