using System;
using Windows.Devices.Gpio;

namespace SprinklerCore
{
    internal class GpioZoneController : ZoneController
    {
        public override bool IsManual { get; internal set; }
        public GpioPin Pin { get; private set; }

        internal GpioZoneController(int pinNumber) : base(pinNumber)
        {
            var gpioController = GpioController.GetDefault();
            var pin = gpioController.OpenPin(pinNumber);
            pin.SetDriveMode(GpioPinDriveMode.Output);
            Pin = pin;
        }

        public override void Start()
        {
            if (Pin.Read() == GpioPinValue.High)
                Pin.Write(GpioPinValue.Low);
        }

        public override void Stop()
        {
            if (Pin.Read() == GpioPinValue.High)
                Pin.Write(GpioPinValue.Low);
        }

        public override bool IsRunning()
        {
            return (Pin.Read() == GpioPinValue.High);
        }
    }
}
