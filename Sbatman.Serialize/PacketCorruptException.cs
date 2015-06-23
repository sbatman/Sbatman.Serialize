using System;

namespace Sbatman.Serialize
{
    internal class PacketCorruptException : Exception
    {
        public PacketCorruptException()
        {
        }

        public PacketCorruptException(String message) : base(message)
        {
        }

        public PacketCorruptException(String message, Exception innerException) : base(message, innerException)
        {
        }
    }
}