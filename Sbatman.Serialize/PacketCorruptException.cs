using System;

namespace Sbatman.Serialize
{
    internal class PacketCorruptException : Exception
    {
        public PacketCorruptException()
        {
        }

        public PacketCorruptException(string message) : base(message)
        {
        }

        public PacketCorruptException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}