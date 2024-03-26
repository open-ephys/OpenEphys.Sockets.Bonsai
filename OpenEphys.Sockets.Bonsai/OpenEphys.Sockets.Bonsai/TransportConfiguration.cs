using System.Xml.Serialization;

namespace OpenEphys.Sockets.Bonsai
{
    [XmlInclude(typeof(TcpServerConfiguration))]
    public abstract class TransportConfiguration
    {
        public string Name { get; set; }

        internal abstract ITransport CreateTransport();
    }
}
