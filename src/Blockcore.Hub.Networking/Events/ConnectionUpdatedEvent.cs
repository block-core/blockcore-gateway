using Blockcore.Platform.Networking.Entities;

namespace Blockcore.Platform.Networking.Events
{
    public class ConnectionUpdatedEvent : BaseEvent
   {
        public HubInfo Data { get; set; }
    }
}
