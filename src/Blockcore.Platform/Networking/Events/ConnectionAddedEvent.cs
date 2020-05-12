using Blockcore.Platform.Networking.Entities;

namespace Blockcore.Platform.Networking.Events
{
    public class ConnectionAddedEvent
    {
        public HubInfo Data { get; set; }
    }
}
