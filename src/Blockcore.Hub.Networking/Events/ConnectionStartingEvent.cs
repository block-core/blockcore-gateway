using Blockcore.Platform.Networking.Entities;

namespace Blockcore.Platform.Networking.Events
{
    public class ConnectionStartingEvent
    {
        public HubInfo Data { get; set; }
    }
}
