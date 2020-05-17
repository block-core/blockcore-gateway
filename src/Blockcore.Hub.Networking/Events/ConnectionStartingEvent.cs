using Blockcore.Platform.Networking.Entities;

namespace Blockcore.Platform.Networking.Events
{
   public class ConnectionStartingEvent : BaseEvent
   {
      public HubInfo Data { get; set; }
   }
}
