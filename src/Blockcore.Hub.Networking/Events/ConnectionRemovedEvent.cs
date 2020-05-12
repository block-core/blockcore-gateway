using Blockcore.Platform.Networking.Entities;

namespace Blockcore.Platform.Networking.Events
{
    public class ConnectionRemovedEvent
    {
        public HubInfo Data { get; set; }
    }
}
