using System.Xml.Serialization;

namespace OpenEphys.Sockets.Bonsai
{
    /// <summary>
    /// Abstract class for transport configurations.
    /// </summary>
    [XmlInclude(typeof(TcpServerConfiguration))]
    public abstract class TransportConfiguration
    {
        /// <summary>
        /// Name of the transport configuration.
        /// </summary>
        public string Name { get; set; }

        internal abstract ITransport CreateTransport();
    }
}
