using System;

namespace OpenEphys.Sockets.Bonsai
{
    public sealed class Message
    {
        public byte[] Data { get; }
        
        public Message(byte[] data)
        {
            Data = data;
        }
    }
}
