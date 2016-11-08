using System;
using Windows.Devices.Gpio;

namespace SprinklerCore
{
    internal class ZoneController
    {
        public bool IsManual { get; internal set; }
        public GpioPin Pin { get; private set; }

        internal ZoneController(int pinNumber)
        {
            var gpioController = GpioController.GetDefault();
            var pin = gpioController.OpenPin(pinNumber);
            pin.SetDriveMode(GpioPinDriveMode.Output);
            Pin = pin;
        }

        public void Start()
        {
            if (Pin.Read() == GpioPinValue.High)
                Pin.Write(GpioPinValue.Low);
        }

        public void Stop()
        {
            if (Pin.Read() == GpioPinValue.High)
                Pin.Write(GpioPinValue.Low);
        }

        public bool IsRunning()
        {
            return (Pin.Read() == GpioPinValue.High);
        }
    }
}
