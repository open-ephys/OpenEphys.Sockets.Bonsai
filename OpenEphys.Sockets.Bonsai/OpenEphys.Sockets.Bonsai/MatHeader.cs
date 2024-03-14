using System.Runtime.InteropServices;

namespace OpenEphys.Sockets.Bonsai
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MatHeader
    {
        public int numBytes;
        public int offset;
        public short bitDepth;
        public int elementSize;
        public int rows;
        public int cols;
    }
}
