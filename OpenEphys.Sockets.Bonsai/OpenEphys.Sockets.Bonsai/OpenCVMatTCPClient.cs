using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using Bonsai;
using OpenCV.Net;

namespace OpenEphys.Sockets.Bonsai
{
    [Combinator]
    [WorkflowElementCategory(ElementCategory.Sink)]
    [Description("Sends a 2D Open CV Mat to a TCP Socket.")]
    public class OpenCVMatTCPClient : Sink<Mat>
    {
        [Description("Address")]
        public string Address { get; set; } = "localhost";

        [Description("Port number. Changing while running has no effect.")]
        public int Port { get; set; } = 5000;

        private TcpListener listener;

        public override IObservable<Mat> Process(IObservable<Mat> source)
        {
            return Observable.Using(
                () =>
                {
                    var address = Address == "localhost" ? "127.0.0.1" : Address;

                    listener = new TcpListener(IPAddress.Parse(address), Port);
                    listener.Start();

                    return listener.AcceptTcpClient().GetStream();
                },
                stream =>
                {
                    return source.Do(value =>
                    {
                        int numBytes = value.ElementSize * value.Cols * value.Rows;

                        var header = new MatHeader
                        {
                            offset = 0,
                            numBytes = numBytes,
                            bitDepth = (short)value.Depth,
                            elementSize = value.ElementSize,
                            numChannels = value.Rows,
                            numSamples = value.Cols
                        };

                        var headerArray = Helpers.SerializeValueType(header);

                        var data = new byte[numBytes + headerArray.Length];

                        Buffer.BlockCopy(headerArray, 0, data, 0, headerArray.Length);
                        Marshal.Copy(value.Data, data, headerArray.Length, numBytes);

                        stream.Write(data, 0, data.Length);

                    }).Finally(
                        () => listener.Stop()
                    );
                });
        }
    }
}
