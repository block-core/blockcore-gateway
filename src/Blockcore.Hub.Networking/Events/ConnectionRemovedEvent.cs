using Blockcore.Platform.Networking.Entities;

namespace Blockcore.Platform.Networking.Events
{
   public class BaseEvent {
      public string EventName { get { return GetType().Name; } }
   }

   public class ConnectionRemovedEvent : BaseEvent
   {
      public HubInfo Data { get; set; }
   }
}
