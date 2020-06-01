using Blockcore.Hub.Networking.Messages;
using Blockcore.Platform.Networking.Messages;

namespace Blockcore.Platform.Networking.Entities
{
   public class Broadcast : BaseEntity
   {
      public string From { get; set; }

      public string Content { get; set; }

      public string To { get; set; }

      public Broadcast()
      {

      }

      public Broadcast(BroadcastMessage message)
      {
         From = message.From;
         Content = message.Content;
         Id = message.Id;
         To = message.To;
      }

      public override BaseMessage ToMessage()
      {
         var msg = new BroadcastMessage
         {
            From = From,
            Content = Content,
            Id = Id,
            To = To
         };

         return msg;
      }
   }
}
