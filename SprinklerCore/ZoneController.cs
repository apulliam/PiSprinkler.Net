using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Foundation.Metadata;

namespace SprinklerCore
{
    public class ZoneController
    {
        [JsonConstructor]
        protected ZoneController(int ZoneNumber, string Name, int PinNumber)
        {
            this.ZoneNumber = ZoneNumber;
            this.Name = Name;
            this.PinNumber = PinNumber;

            if (ApiInformation.IsTypePresent("Windows.Devices.Gpio"))
            {
                var gpioController = GpioController.GetDefault();
                var pin = gpioController.OpenPin(this.PinNumber);
                pin.SetDriveMode(GpioPinDriveMode.Output);
            }
        }
        public int ZoneNumber { get; protected set;  }

        public string Name { get; protected set; }

        public int PinNumber { get; protected set; }

        public bool IsManual { get;  set; }

        private bool _isRunning = false;

        private GpioPin Pin { get; set; }

        public void Start()
        {
            if (ApiInformation.IsTypePresent("Windows.Devices.Gpio"))
            {
                if (Pin.Read() == GpioPinValue.High)
                    Pin.Write(GpioPinValue.Low);
            }
            else
                _isRunning = true;
        }

        public void Stop()
        {
            IsManual = false;
            if (ApiInformation.IsTypePresent("Windows.Devices.Gpio"))
            {
                if (Pin.Read() == GpioPinValue.High)
                    Pin.Write(GpioPinValue.Low);
            }
            else
                _isRunning = false;
        }

        public bool IsRunning()
        {
            if (ApiInformation.IsTypePresent("Windows.Devices.Gpio"))
            {
                return (Pin.Read() == GpioPinValue.High);
            }
            else
                return _isRunning;
        }
    }

}
