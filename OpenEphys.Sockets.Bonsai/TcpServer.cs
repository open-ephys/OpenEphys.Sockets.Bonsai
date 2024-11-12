using System.ComponentModel;

namespace OpenEphys.Sockets.Bonsai
{
    /// <summary>
    /// Creates a TCP server to send data through.
    /// </summary>
    [Description("Creates a TCP server.")]
    public class TcpServer : CreateTransport
    {
        readonly TcpServerConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpServer"/> class.
        /// </summary>
        public TcpServer()
            : this(new TcpServerConfiguration())
        {
        }

        private TcpServer(TcpServerConfiguration configuration)
            : base(configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// The port on which to listen for incoming connection attempts.
        /// </summary>
        [Description("The port on which to listen for incoming connection attempts.")]
        public int Port
        {
            get { return configuration.Port; }
            set { configuration.Port = value; }
        }

        /// <summary>
        /// The address of the host to connect to.
        /// </summary>
        /// <remarks>Can be "localhost" for local communication.</remarks>
        [Description("The address of the host to connect to. Can be \"localhost\" for local communication.")]
        public string Address
        {
            get { return configuration.Address; }
            set { configuration.Address = value; }
        }
    }
}
