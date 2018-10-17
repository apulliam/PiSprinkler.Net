namespace SprinklerDotNet
{
    public abstract class ZoneBase
    {
        public int ZoneNumber { get; private set; }

        public string Name { get; private set; }

        public int PinNumber { get; private set; }

        public ZoneBase(int zoneNumber, string name, int pinNumber)
        {
            this.ZoneNumber = zoneNumber;
            this.Name = name;
            this.PinNumber = pinNumber;
        }

        public abstract void Start();

        public abstract void Stop();

        public abstract bool IsRunning { get; }
    }
}
