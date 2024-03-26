using System;

namespace OpenEphys.Sockets.Bonsai
{
    interface ITransport : IDisposable
    {
        void SendPacket(Action<DataWriter> writePacket);
    }
}
