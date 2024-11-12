using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace OpenEphys.Sockets.Bonsai
{
    /// <summary>
    /// Create a transport channel to send data through.
    /// </summary>
    [DefaultProperty(nameof(Name))]
    public abstract class CreateTransport : Source<IDisposable>, INamedElement
    {
        readonly TransportConfiguration configuration;

        internal CreateTransport(TransportConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// The name of the communication channel to reserve.
        /// </summary>
        [Description("The name of the communication channel to reserve.")]
        public string Name
        {
            get { return configuration.Name; }
            set { configuration.Name = value; }
        }

        /// <summary>
        /// Reserves the connection using the given <see cref="Name"/>, enabling it for transport.
        /// </summary>
        /// <returns></returns>
        public override IObservable<IDisposable> Generate()
        {
            return Observable.Using(
                () => TransportManager.ReserveConnection(configuration.Name, configuration),
                connection => Observable.Return(connection.Transport).Concat(Observable.Never(connection.Transport)));
        }
    }
}
