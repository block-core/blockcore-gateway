using MessagePack;

namespace Blockcore.Platform.Networking.Messages
{

    [MessagePackObject]
    public class AckMessage : BaseMessage
    {
        public override ushort Command => MessageTypes.ACK;

        [Key(1)]
        public long RecipientId { get; set; }

        [Key(2)]
        public bool Response { get; set; }

        public AckMessage()
        {

        }

        public AckMessage(long senderId)
        {
            Id = senderId;
        }
    }
}
