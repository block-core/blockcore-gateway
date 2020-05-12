using MessagePack;

namespace Blockcore.Platform.Networking.Messages
{
    [MessagePackObject]
    public class TestMessage : BaseMessage
    {
        public override ushort Command => MessageTypes.TEST;

        [Key(1)]
        public string CustomString { get; set; }

        [Key(2)]
        public int CustomInt { get; set; }

        [Key(3)]
        public string Endpoint { get; set; }

        protected bool Equals(TestMessage other)
        {
            return string.Equals(CustomString, other.CustomString) && CustomInt == other.CustomInt;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TestMessage)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((CustomString != null ? CustomString.GetHashCode() : 0) * 397) ^ CustomInt;
            }
        }
    }
}
