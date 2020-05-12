using MessagePack;

namespace Blockcore.Platform.Networking.Messages
{
    [MessagePackObject]
    public class NotificationMessage : BaseMessage
    {
        public override ushort Command => MessageTypes.NOTIFY;

        [Key(1)]
        public NotificationsTypes Type { get; set; }

        [Key(2)]
        public object Tag { get; set; }

        public NotificationMessage()
        {

        }

        public NotificationMessage(NotificationsTypes type, object tag)
        {
            Type = type;
            Tag = tag;
        }
    }
}
