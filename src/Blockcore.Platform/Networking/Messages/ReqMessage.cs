using MessagePack;

namespace Blockcore.Platform.Networking.Messages
{
    [MessagePackObject]
    public class ReqMessage : BaseMessage
    {
        public override ushort Command => MessageTypes.REQ;

        [Key(1)]
        public long RecipientId { get; set; }

        public ReqMessage()
        {

        }

        public ReqMessage(long senderId, long recipientID)
        {
            Id = senderId;
            RecipientId = recipientID;
        }
    }
}
