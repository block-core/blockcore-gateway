using Blockcore.Platform.Networking.Messages;

namespace Blockcore.Platform.Networking.Entities
{
    public class Message : BaseEntity
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Content { get; set; }
        public long RecipientId { get; set; }

        public Message(string from, string to, string content)
        {
            From = from;
            To = to;
            Content = content;
        }

        public Message(MessageMessage message)
        {
            this.From = message.From;
            this.To = message.To;
            this.Content = message.Content;
            this.Id = message.Id;
            this.RecipientId = message.RecipientId;
        }

        public override BaseMessage ToMessage()
        {
            var msg = new MessageMessage();

            this.From = From;
            this.To = To;
            this.Content = Content;
            this.Id = Id;
            this.RecipientId = RecipientId;

            return msg;
        }
    }
}
