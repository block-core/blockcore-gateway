using Blockcore.Platform.Networking.Messages;

namespace Blockcore.Platform.Networking.Entities
{
    public class Ack : BaseEntity
    {
        public long RecipientId { get; set; }

        public bool Response { get; set; }

        public Ack(long senderId)
        {
            Id = senderId;
        }

        public Ack(AckMessage message)
        {
            this.Id = message.Id;
            this.RecipientId = message.RecipientId;
            this.Response = message.Response;
        }

        public override BaseMessage ToMessage()
        {
            var msg = new AckMessage(this.Id);

            msg.RecipientId = this.RecipientId;
            msg.Response = this.Response;

            return msg;
        }
    }
}
