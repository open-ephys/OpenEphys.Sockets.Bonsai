using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Bonsai;
using Bonsai.Reactive;
using OpenCV.Net;

namespace OpenEphys.Bonsai.EphysSocket
{
    [Combinator]
    [WorkflowElementCategory(ElementCategory.Sink)]
    [Description("Sends a 2D Open CV Mat to a datagram (UDP) socket.")]
    public class OpenCVMatUDPClient : Sink<Mat>
    {
        [Description("Address")]
        public string Address { get; set; } = "localhost";

        [Description("Port number. Changing while running has no effect.")]
        public int Port { get; set; } = 5000;

        internal int MaxPacketSize = 65506; // NB: Max bytes == 65506 for IPv4, not including headers

        // Receiver has to provide number of rows and cols
        public unsafe override IObservable<Mat> Process(IObservable<Mat> source)
        {
            return Observable.Using(
                () =>
                {
                    var u = new UdpClient();
                    u.Connect(Address, Port);
                    return u;
                },
                u =>
                {
                    return source.Do(value =>
                    {
                        //Mat value_transposed = new Mat(value.Cols, value.Rows, value.Depth, value.Channels);
                        //CV.Transpose(value, value_transposed);
                        //value.Dispose();
                        //value = value_transposed;

                        var headerSize = sizeof(int) * 2;
                        var totalPacketSize = value.ElementSize * value.Cols * value.Rows;
                        var packetRatio = (totalPacketSize + headerSize) / MaxPacketSize + 1;
                        var packetSize = (totalPacketSize / packetRatio) + headerSize;

                        for (int i = 0; i < packetRatio; i++)
                        {
                            var offset = i * (packetSize - headerSize);
                            byte[] offsetBytes = BitConverter.GetBytes(offset);

                            int numBytes = packetSize - headerSize;
                            byte[] numBytesBytes = BitConverter.GetBytes(numBytes);

                            var data = new byte[numBytes + headerSize];

                            System.Buffer.BlockCopy(offsetBytes, 0, data, 0, sizeof(int));
                            System.Buffer.BlockCopy(numBytesBytes, 0, data, sizeof(int), sizeof(int));

                            for (int j = 0; j < numBytes; j++)
                            {
                                data[headerSize + j] = ((byte*)value.Data)[j + offset];
                            }

                            u.Send(data, numBytes + headerSize);
                        }
                    });
                });
        }
    }
}
