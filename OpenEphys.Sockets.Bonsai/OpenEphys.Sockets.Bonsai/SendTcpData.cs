using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using Bonsai;
using OpenCV.Net;

namespace OpenEphys.Sockets.Bonsai
{
    [WorkflowElementCategory(ElementCategory.Sink)]
    [Description("Sends a 2D Open CV Mat to a TCP Socket.")]
    public class SendTcpData : Sink<Mat>
    {
        [TypeConverter(typeof(ConnectionNameConverter))]
        [Description("The name of the communication channel to send data over.")]
        public string Connection { get; set; }  

        public override IObservable<Mat> Process(IObservable<Mat> source)
        {
            return Observable.Using(
                () => TransportManager.ReserveConnection(Connection),
                connection =>
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

                        connection.Transport.SendPacket(writer => writer.Write(data));
                    });
                });
        }
    }
}
