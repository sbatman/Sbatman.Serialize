using System;

namespace Sbatman.Serialize
{
    internal class NotAPacketException : Exception
    {
        public NotAPacketException()
        {
        }

        public NotAPacketException(String message) : base(message)
        {
        }

        public NotAPacketException(String message, Exception innerException) : base(message, innerException)
        {
        }
    }
}