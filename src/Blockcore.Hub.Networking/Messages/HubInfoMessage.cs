using MessagePack;
using System.Collections.Generic;

namespace Blockcore.Platform.Networking.Messages
{
    [MessagePackObject]
    public class HubInfoMessage : BaseMessage
    {
        public override ushort Command => MessageTypes.INFO;

        [Key(1)]
        public string Name { get; set; }

        [Key(2)]
        public string ExternalEndpoint { get; set; }

        [Key(3)]
        public string InternalEndpoint { get; set; }

        [Key(4)]
        // TODO: Maybe this should simply be string?
        public ConnectionTypes ConnectionType { get; set; }

        [Key(5)]
        public List<string> InternalAddresses = new List<string>();

        [Key(6)]
        public string FirstName { get; set; }
    }
}
