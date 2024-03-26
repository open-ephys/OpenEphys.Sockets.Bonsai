using System;
using System.IO;
using System.Net;
using System.Text;

namespace OpenEphys.Sockets.Bonsai
{
    class DataWriter : BinaryWriter
    {
        public DataWriter(Stream output) 
            : this(output, false)
        {
        }

        public DataWriter(Stream output, bool leaveOpen)
            : base(output, Encoding.UTF8, leaveOpen)
        { 
        }

        public override void Write(byte[] data)
        {
            base.Write(data);
        }
    }
}
