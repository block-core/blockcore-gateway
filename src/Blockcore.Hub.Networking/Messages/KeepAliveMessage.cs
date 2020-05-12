using MessagePack;

namespace Blockcore.Platform.Networking.Messages
{
    [MessagePackObject]
    public class KeepAliveMessage : BaseMessage
    {
        public override ushort Command => MessageTypes.KEEPALIVE;
    }
}
