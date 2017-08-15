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
        private static GpioPinValue ZoneOff = GpioPinValue.High;
        private static GpioPinValue ZoneOn = GpioPinValue.Low;

        [JsonConstructor]
        protected ZoneController(int ZoneNumber, string Name, int PinNumber)
        {
            this.ZoneNumber = ZoneNumber;
            this.Name = Name;
            this.PinNumber = PinNumber;

            var gpioController = GpioController.GetDefault();
            if (gpioController != null)
            { 
                Pin = gpioController.OpenPin(this.PinNumber);
                Pin.SetDriveMode(GpioPinDriveMode.Output);
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
           
            if (Pin != null)
            { 
                var state = Pin.Read();
                if (state == ZoneOff)
                    Pin.Write(ZoneOn);
            }
            else
                _isRunning = true;
        }

        public void Stop()
        {
            IsManual = false;
            if (Pin != null)
            {
                if (Pin.Read() == ZoneOn)
                    Pin.Write(ZoneOff);
            }
            else
                _isRunning = false;
        }

        public bool IsRunning
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
