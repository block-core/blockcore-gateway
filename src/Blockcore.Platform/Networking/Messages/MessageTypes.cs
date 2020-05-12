namespace Blockcore.Platform.Networking.Messages
{
    // TODO: Migrate to use a fixed-length string based message header.
    public class MessageTypes
    {
        public static ushort ACK = 0;
        public static ushort MSG = 1;
        public static ushort INFO = 2;
        public static ushort KEEPALIVE = 3;
        public static ushort NOTIFY = 4;
        public static ushort REQ = 5;
        public static ushort TEST = 999;
    }
}
