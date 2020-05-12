using Blockcore.Platform.Networking.Messages;

namespace Blockcore.Platform.Networking.Entities
{
    public class Notification : BaseEntity
    {
        public NotificationsTypes Type { get; set; }

        public object Tag { get; set; }

        public Notification(NotificationsTypes type, object tag)
        {
            Type = type;
            Tag = tag;
        }

        public override BaseMessage ToMessage()
        {
            var msg = new NotificationMessage(Type, Tag);
            return msg;
        }
    }
}
