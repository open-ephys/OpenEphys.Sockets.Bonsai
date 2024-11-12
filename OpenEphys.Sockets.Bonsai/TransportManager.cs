using System;
using System.Collections.Generic;
using System.Reactive.Disposables;

namespace OpenEphys.Sockets.Bonsai
{
    internal static class TransportManager
    {
        static readonly Dictionary<string, Tuple<ITransport, RefCountDisposable>> openConnections = new();
        static readonly object openConnectionsLock = new();

        public static TransportDisposable ReserveConnection(string name)
        {
            return ReserveConnection(name, null);
        }

        internal static TransportDisposable ReserveConnection(string name, TransportConfiguration transportConfiguration)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("A connection name must be specified.", nameof(name));
            }

            Tuple<ITransport, RefCountDisposable> connection;
            lock (openConnectionsLock)
            {
                if (!openConnections.TryGetValue(name, out connection))
                {
                    var transport = transportConfiguration.CreateTransport();
                    var dispose = Disposable.Create(() =>
                    {
                        transport.Dispose();
                        openConnections.Remove(name);
                    });

                    var refCount = new RefCountDisposable(dispose);
                    connection = Tuple.Create(transport, refCount);
                    openConnections.Add(name, connection);
                    return new TransportDisposable(transport, refCount);
                }
            }

            return new TransportDisposable(connection.Item1, connection.Item2.GetDisposable());
        }
    }
}
