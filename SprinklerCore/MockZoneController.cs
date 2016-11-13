using System;
using Windows.Devices.Gpio;

namespace SprinklerCore
{
    internal class MockZoneController : ZoneController
    {
        public override bool IsManual { get; internal set; }
        private bool _isRunning = false;
     
        internal MockZoneController(int pinNumber) : base(pinNumber)
        {
         
        }

        public override void Start()
        {
            _isRunning = true;
        }

        public override void Stop()
        {
            _isRunning = false;
        }

        public override bool IsRunning()
        {
            return _isRunning;
        }
    }
}
