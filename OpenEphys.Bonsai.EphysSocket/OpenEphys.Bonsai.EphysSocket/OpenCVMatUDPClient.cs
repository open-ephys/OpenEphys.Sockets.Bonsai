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
    [Description("Sends a 2D Open CV Mat to a datagram (UDP) socket. Element type is preserved.")]
    public class OpenCVMatUDPClient : Sink<Mat>
    {
        [Description("Address")]
        public string Address { get; set; } = "localhost";

        [Description("Port number. Changing while running has no effect.")]
        public int Port { get; set; } = 5000;

        const int MaxPacketSize = 65506; // NB: Max bytes == 65506 for IPv4, not including headers

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
                        const int headerSize = 22;

                        int matrixSize = value.ElementSize * value.Cols * value.Rows;
                        int packetsToSend = (matrixSize / MaxPacketSize) + 1;
                        int maxPacketSize = MaxPacketSize - headerSize;

                        byte[] bitDepthBytes = BitConverter.GetBytes((short)value.Depth);
                        byte[] elementSizeBytes = BitConverter.GetBytes(value.ElementSize);
                        byte[] numChannelsBytes = BitConverter.GetBytes(value.Rows);
                        byte[] numSamplesBytes = BitConverter.GetBytes(value.Cols);

                        for (int i = 0; i < packetsToSend; i++)
                        {
                            int offset = i * maxPacketSize;
                            byte[] offsetBytes = BitConverter.GetBytes(offset);

                            int numBytes = matrixSize < maxPacketSize ? matrixSize : 
                                            matrixSize - offset < maxPacketSize ? matrixSize - offset : maxPacketSize;
                            byte[] numBytesBytes = BitConverter.GetBytes(numBytes);

                            var packet = new byte[numBytes + headerSize];

                            System.Buffer.BlockCopy(offsetBytes, 0, packet, 0, 4);
                            System.Buffer.BlockCopy(numBytesBytes, 0, packet, 4, 4);
                            System.Buffer.BlockCopy(bitDepthBytes, 0, packet, 8, 2);
                            System.Buffer.BlockCopy(elementSizeBytes, 0, packet, 10, 4);
                            System.Buffer.BlockCopy(numChannelsBytes, 0, packet, 14, 4);
                            System.Buffer.BlockCopy(numSamplesBytes, 0, packet, 18, 4);

                            Marshal.Copy(value.Data + offset, packet, headerSize, numBytes);

                            u.Send(packet, numBytes + headerSize);
                        }
                    });
                });
        }
    }
}
