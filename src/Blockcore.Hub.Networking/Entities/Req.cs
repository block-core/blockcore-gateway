using Blockcore.Platform.Networking.Messages;

namespace Blockcore.Platform.Networking.Entities
{
    public class Req : BaseEntity
    {
        public long RecipientId { get; set; }

        public Req(long senderId, long recipientId)
        {
            Id = senderId;
            RecipientId = recipientId;
        }

        public Req(ReqMessage message)
        {
            this.Id = message.Id;
            this.RecipientId = message.RecipientId;
        }

        public override BaseMessage ToMessage()
        {
            var msg = new ReqMessage(Id, RecipientId);
            return msg;
        }
    }
}
