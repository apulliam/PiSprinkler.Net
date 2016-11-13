using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprinklerCore
{
    public abstract class ZoneController
    {
        protected ZoneController(int pinNumber)
        {
        }
        public abstract bool IsManual { get; internal set; }

        public abstract void Start();

        public abstract void Stop();

        public abstract bool IsRunning();
    }

}
