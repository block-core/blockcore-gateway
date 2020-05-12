using Blockcore.Platform.Networking.Messages;

namespace Blockcore.Platform.Networking.Entities
{
    public class KeepAlive : BaseEntity
    {
        public override BaseMessage ToMessage()
        {
            return new KeepAliveMessage() { Id = Id };
        }
    }
}
