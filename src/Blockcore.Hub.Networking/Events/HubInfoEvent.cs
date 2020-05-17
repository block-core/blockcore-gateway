using Blockcore.Platform.Networking.Messages;

namespace Blockcore.Platform.Networking.Events
{
   public class HubInfoEvent : BaseEvent
   {
      public HubInfoMessage Data { get; set; }
   }
}
