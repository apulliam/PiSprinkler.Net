using System;

namespace SprinklerDotNet
{
    public class SprinklerControllerException : Exception
    {
        public SprinklerControllerException()
        {
        }

        public SprinklerControllerException(string message) : base(message)
        {
        }

        public SprinklerControllerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}