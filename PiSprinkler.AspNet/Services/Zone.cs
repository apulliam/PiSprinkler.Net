using Newtonsoft.Json;
using SprinklerDotNet;
using System.Threading.Tasks;
using Unosquare.RaspberryIO.Gpio;

namespace PiSprinkler.AspNet.Services
{
    internal class Zone : ZoneBase
    {
        private static GpioPinValue ZoneOff = GpioPinValue.High;
        private static GpioPinValue ZoneOn = GpioPinValue.Low;

        [JsonConstructor]
        public Zone(int ZoneNumber, string Name, int PinNumber) : base(ZoneNumber, Name, PinNumber)
        {
            try
            { 
            var gpioController = GpioController.Instance;
            if (gpioController != null)
            {
                Pin = gpioController.Pins[this.PinNumber];
                Pin.PinMode = GpioPinDriveMode.Output;
            }
            }
            catch
            { }
        }
     
        private bool _isRunning = false;

        private GpioPin Pin { get; set; }

        public override void Start()
        {
           
            if (Pin != null)
            { 
                var state = Pin.ReadValue();
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
                if (Pin.ReadValue() == ZoneOn)
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
                    return (Pin.ReadValue() == ZoneOn);
                }
                else
                    return _isRunning;
            }
        }
    }

}
