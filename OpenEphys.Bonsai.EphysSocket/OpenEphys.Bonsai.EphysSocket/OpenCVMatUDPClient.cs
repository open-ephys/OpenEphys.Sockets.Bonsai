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

                        var header_size = sizeof(int) * 2;
                        var total_packet_size = value.ElementSize * value.Cols * value.Rows;
                        var packet_ratio = (total_packet_size + header_size) / MaxPacketSize + 1;
                        var packet_size = (total_packet_size / packet_ratio) + header_size;

                        for (int i = 0; i < packet_ratio; i++)
                        {
                            var offset = i * (packet_size - header_size);
                            byte[] offset_bytes = BitConverter.GetBytes(offset);

                            int num_bytes = packet_size - header_size;
                            byte[] num_bytes_bytes = BitConverter.GetBytes(num_bytes);

                            var data = new byte[num_bytes + header_size];

                            System.Buffer.BlockCopy(offset_bytes, 0, data, 0, sizeof(int));
                            System.Buffer.BlockCopy(num_bytes_bytes, 0, data, sizeof(int), sizeof(int));

                            for (int j = 0; j < num_bytes; j++)
                            {
                                data[header_size + j] = ((byte*)value.Data)[j + offset];
                            }

                            u.Send(data, num_bytes + header_size);
                        }
                    });
                });
        }
    }
}
