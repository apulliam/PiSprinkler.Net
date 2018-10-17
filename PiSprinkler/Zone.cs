using Newtonsoft.Json;
using SprinklerDotNet;
using Windows.Devices.Gpio;

namespace PiSprinkler
{
    internal class Zone : ZoneBase
    {
        private static GpioPinValue ZoneOff = GpioPinValue.High;
        private static GpioPinValue ZoneOn = GpioPinValue.Low;

        private bool _isRunning = false;

        [JsonConstructor]
        public Zone(int zoneNumber, string name, int pinNumber) : base(zoneNumber, name, pinNumber)
        {

            var gpioController = GpioController.GetDefault();
            if (gpioController != null)
            {
                Pin = gpioController.OpenPin(this.PinNumber);
                Pin.SetDriveMode(GpioPinDriveMode.Output);
            }
        }

        private GpioPin Pin { get; set; }

        public override void Start()
        {
           
            if (Pin != null)
            { 
                var state = Pin.Read();
                if (state == ZoneOff)
                    Pin.Write(ZoneOn);
            }
            else
                _isRunning = true;
        }

        public override void Stop()
        {
            if (Pin != null)
            {
                if (Pin.Read() == ZoneOn)
                    Pin.Write(ZoneOff);
            }
            else
                _isRunning = false;
        }

        public override bool IsRunning
        {
            get
            {
                if (Pin != null)
                {
                    return (Pin.Read() == ZoneOn);
                }
                else
                    return _isRunning;
            }
        }
    }

}
