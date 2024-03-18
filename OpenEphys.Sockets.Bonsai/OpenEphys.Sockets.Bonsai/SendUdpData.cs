using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using Bonsai;
using OpenCV.Net;

namespace OpenEphys.Sockets.Bonsai
{
    [WorkflowElementCategory(ElementCategory.Sink)]
    [Description("Sends a 2D Open CV Mat to a datagram (UDP) socket. Element type is preserved.")]
    public class SendUdpData : Sink<Mat>
    {
        [Description("The name of the communication channel to send data over.")]
        public string Connection { get; set; }

        const int MaxPacketSize = 65506; // NB: Max bytes == 65506 for IPv4, not including headers

        public unsafe override IObservable<Mat> Process(IObservable<Mat> source)
        {
            return Observable.Using(
                () => TransportManager.ReserveConnection(Connection),
                connection =>
                {
                    return source.Do(value =>
                    {
                        const int headerSize = 22;

                        int matrixSize = value.ElementSize * value.Cols * value.Rows;
                        int packetsToSend = (matrixSize / MaxPacketSize) + 1;
                        int maxPacketSize = MaxPacketSize - headerSize;

                        var header = new MatHeader
                        {
                            offset = 0,
                            numBytes = matrixSize,
                            bitDepth = (short)value.Depth,
                            elementSize = value.ElementSize,
                            numChannels = value.Rows,
                            numSamples = value.Cols
                        };

                        var data = new byte[matrixSize + headerSize];

                        for (int i = 0; i < packetsToSend; i++)
                        {
                            header.offset = i * maxPacketSize;

                            int numBytes = matrixSize < maxPacketSize ? matrixSize : 
                                            matrixSize - header.offset < maxPacketSize ? matrixSize - header.offset : maxPacketSize;

                            var headerArray = Helpers.SerializeValueType(header);

                            Buffer.BlockCopy(headerArray, 0, data, 0, headerArray.Length);
                            Marshal.Copy(value.Data + header.offset, data, headerSize, numBytes);

                            connection.Transport.SendPacket(writer => writer.Write(data));
                        }
                    });
                });
        }
    }
}
