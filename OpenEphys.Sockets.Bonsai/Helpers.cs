

namespace OpenEphys.Sockets.Bonsai
{
    internal static class Helpers
    {
        internal static unsafe byte[] SerializeValueType<T>(in T value) where T : unmanaged
        {
            byte[] result = new byte[sizeof(T)];
            fixed (byte* dst = result)
                *(T*)dst = value;
            return result;
        }
    }
}
