using Bonsai;
using System;
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
            if (Address == null)
            {
                throw new ArgumentException("Address cannot be null", "Address");
            }

            if (Port < 1024 || Port > 65535)
            {
                throw new ArgumentException("Invalid port number given. Must be between 1024 and 65535", "Port");
            }

            var address = Address == "localhost" ? "127.0.0.1" : Address;

            try
            {
                IPAddress parsedAddress = IPAddress.Parse(address);

                var listener = new TcpListener(parsedAddress, Port);
                return new TcpServerTransport(listener);
            }
            catch (FormatException)
            {
                throw new ArgumentException("Address is not formatted correctly. ", "Address");
            }
        }
    }
}
