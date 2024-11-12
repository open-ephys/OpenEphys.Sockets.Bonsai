namespace OpenEphys.Sockets.Bonsai
{
    /// <summary>
    /// Class to hold the message to send as a byte array.
    /// </summary>
    public sealed class Message
    {
        /// <summary>
        /// The content of the message as a byte array.
        /// </summary>
        public byte[] Data { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="data">Contents of the message as a byte array.</param>
        public Message(byte[] data)
        {
            Data = data;
        }
    }
}
