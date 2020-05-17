using Blockcore.Platform.Networking.Entities;

namespace Blockcore.Platform.Networking.Events
{
   public class ConnectionAddedEvent : BaseEvent
   {
      public HubInfo Data { get; set; }
   }
}
