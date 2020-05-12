using MessagePack;

namespace Blockcore.Platform.Networking.Messages
{
    [MessagePackObject]
    public class MessageMessage: BaseMessage
    {
        public override ushort Command => MessageTypes.MSG;

        [Key(1)]
        public string From { get; set; }

        [Key(2)]
        public string To { get; set; }

        [Key(3)]
        public string Content { get; set; }

        [Key(4)]
        public long RecipientId { get; set; }

        public MessageMessage()
        {

        }

        public MessageMessage(string from, string to, string content)
        {
            From = from;
            To = to;
            Content = content;
        }
    }
}
