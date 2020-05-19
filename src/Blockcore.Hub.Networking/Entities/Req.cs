using Blockcore.Platform.Networking.Messages;

namespace Blockcore.Platform.Networking.Entities
{
   public class Req : BaseEntity
   {
      public string RecipientId { get; set; }

      public Req(string senderId, string recipientId)
      {
         Id = senderId;
         RecipientId = recipientId;
      }

      public Req(ReqMessage message)
      {
         Id = message.Id;
         RecipientId = message.RecipientId;
      }

      public override BaseMessage ToMessage()
      {
         var msg = new ReqMessage(Id, RecipientId);
         return msg;
      }
   }
}
