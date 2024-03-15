using System.ComponentModel;

namespace OpenEphys.Sockets.Bonsai
{
    [Description("Creates a TCP server.")]
    public class CreateTcpServer : CreateTransport
    {
        readonly TcpServerConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTcpServer"/> class.
        /// </summary>
        public CreateTcpServer()
            : this(new TcpServerConfiguration())
        {
        }

        private CreateTcpServer(TcpServerConfiguration configuration)
            : base(configuration)
        {
            this.configuration = configuration;
        }

        [Description("The port on which to listen for incoming connection attempts.")]
        public int Port
        {
            get { return configuration.Port; }
            set { configuration.Port = value; }
        }

        [Description("The address of the host to connect to. Can be \"localhost\" for local communication.")]
        public string Address
        {
            get { return configuration.Address; }
            set { configuration.Address = value; }
        }
    }
}
