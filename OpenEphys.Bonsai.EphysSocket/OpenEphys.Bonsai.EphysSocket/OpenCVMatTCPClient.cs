using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using Bonsai;
using OpenCV.Net;

namespace OpenEphys.Bonsai.EphysSocket
{
    [Combinator]
    [WorkflowElementCategory(ElementCategory.Sink)]
    [Description("Sends a 2D Open CV Mat to a TCP Socket. Element type is preserved.")]
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
                    if (Address == "localhost")
                    {
                        Address = "127.0.0.1";
                    }

                    listener = new TcpListener(IPAddress.Parse(Address), Port);
                    listener.Start();

                    return Stream.Null;
                },
                stream =>
                {
                    return source.Do(value =>
                    {
                        if (stream == Stream.Null)
                        {
                            if (listener.Pending())
                            {
                                stream = listener.AcceptTcpClient().GetStream();
                            }
                            else
                            {
                                return;
                            }
                        }
                        const int headerSize = 22;

                        int numBytes = value.ElementSize * value.Cols * value.Rows;
                        byte[] numBytesBytes = BitConverter.GetBytes(numBytes);

                        int offset = 0;
                        byte[] offsetBytes = BitConverter.GetBytes(offset);

                        byte[] bitDepthBytes = BitConverter.GetBytes((short)value.Depth);
                        byte[] elementSizeBytes = BitConverter.GetBytes(value.ElementSize);
                        byte[] numChannelsBytes = BitConverter.GetBytes(value.Rows);
                        byte[] numSamplesBytes = BitConverter.GetBytes(value.Cols);

                        var data = new byte[numBytes + headerSize];

                        Buffer.BlockCopy(offsetBytes, 0, data, 0, 4);
                        Buffer.BlockCopy(numBytesBytes, 0, data, 4, 4);
                        Buffer.BlockCopy(bitDepthBytes, 0, data, 8, 2);
                        Buffer.BlockCopy(elementSizeBytes, 0, data, 10, 4);
                        Buffer.BlockCopy(numChannelsBytes, 0, data, 14, 4);
                        Buffer.BlockCopy(numSamplesBytes, 0, data, 18, 4);

                        Marshal.Copy(value.Data + offset, data, headerSize, numBytes);

                        try
                        {
                            stream.Write(data, 0, data.Length);
                        }
                        catch (IOException) // NB: Socket was closed
                        {
                            stream.Dispose();
                            stream = Stream.Null;
                        }
                    }).Finally(
                        () => listener.Stop()
                    );
                });
        }
    }
}
