using Newtonsoft.Json;
using System.Device.Gpio;

namespace SprinklerCore
{
    public class ZoneController
    {
        private static PinValue ZoneOff = PinValue.High;
        private static PinValue ZoneOn = PinValue.Low;

        [JsonConstructor]
        protected ZoneController(int ZoneNumber, string Name, int PinNumber)
        {
            this.ZoneNumber = ZoneNumber;
            this.Name = Name;
            this.PinNumber = PinNumber;

            var gpioController = new GpioController();
            
            gpioController.OpenPin(this.PinNumber);
            gpioController.SetPinMode(this.PinNumber, PinMode.Output);

        }
        public int ZoneNumber { get; protected set; }

        public string Name { get; protected set; }

        public int PinNumber { get; protected set; }

        private GpioController GpioController { get; set; }

        private bool _isRunning = false;

        //private GpioPin Pin { get; set; }

        public void Start()
        {
           
            //if (Pin != null)
            //{ 
                var state = GpioController.Read(PinNumber);
                if (state == ZoneOff)
                    GpioController.Write(PinNumber, ZoneOn);
            //}
            //else
            //    _isRunning = true;
        }

        public void Stop()
        {
            //if (Pin != null)
            //{
                if (GpioController.Read(PinNumber) == ZoneOn)
                    GpioController.Write(PinNumber, ZoneOff);
            //}
            //else
            //    _isRunning = false;
        }

        public bool IsRunning
        {
            get
            {
                //if (Pin != null)
                //{
                    return (GpioController.Read(PinNumber) == ZoneOn);
                //}
                //else
                //    return _isRunning;
            }
        }
    }

}
