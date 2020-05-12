using Blockcore.Platform.Networking.Messages;

namespace Blockcore.Platform.Networking.Entities
{
    public interface IBaseEntity
    {
        public long Id { get; set; }

        public ushort Command { get; }

        public BaseMessage ToMessage();
    }
}
