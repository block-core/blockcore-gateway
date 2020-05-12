using MessagePack;

namespace Blockcore.Platform.Networking.Messages
{
    public abstract class BaseMessage
    {
        public BaseMessage()
        {

        }

        [IgnoreMember]
        public abstract ushort Command { get; }

        [Key(0)]
        public long Id { get; set; }
    }

    //public interface BaseMessage
    //{ 
    //    public long Id { get; set; }

    //    public ushort Command { get; }
    //}
}
