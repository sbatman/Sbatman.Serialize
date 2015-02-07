using System;

namespace Sbatman.Serialize
{
    internal class NotAPacketException : Exception
    {
        public NotAPacketException()
        {
        }

        public NotAPacketException(string message) : base(message)
        {
        }

        public NotAPacketException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}