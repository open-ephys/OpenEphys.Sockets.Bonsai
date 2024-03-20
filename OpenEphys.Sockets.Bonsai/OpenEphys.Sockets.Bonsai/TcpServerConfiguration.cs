using Bonsai;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;

namespace OpenEphys.Sockets.Bonsai
{
    public class TcpServerConfiguration : TransportConfiguration
    {
        public string Address;
        public int Port;

        internal override ITransport CreateTransport()
        {
            var address = Address == "localhost" ? "127.0.0.1" : Address;

            var listener = new TcpListener(IPAddress.Parse(address), Port);
            return new TcpServerTransport(listener);
        }
    }
}
