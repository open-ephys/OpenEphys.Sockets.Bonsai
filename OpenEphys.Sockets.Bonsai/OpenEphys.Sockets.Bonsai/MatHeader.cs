using System.Runtime.InteropServices;

namespace OpenEphys.Sockets.Bonsai
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct MatHeader
    {
        public int offset;
        public int numBytes;
        public short bitDepth;
        public int elementSize;
        public int numChannels;
        public int numSamples;
    }
}
