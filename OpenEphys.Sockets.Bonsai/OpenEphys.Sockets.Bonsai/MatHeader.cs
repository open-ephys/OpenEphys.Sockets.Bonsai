using System.Runtime.InteropServices;

namespace OpenEphys.Sockets.Bonsai
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MatHeader
    {
        public int offset;
        public int numBytes;
        public int bitDepth;
        public int elementSize;
        public int numChannels;
        public int numSamples;
    }
}
