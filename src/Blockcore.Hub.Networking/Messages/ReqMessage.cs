using MessagePack;

namespace Blockcore.Platform.Networking.Messages
{
   [MessagePackObject]
   public class ReqMessage : BaseMessage
   {
      public override ushort Command => MessageTypes.REQ;

      [Key(1)]
      public string RecipientId { get; set; }

      public ReqMessage()
      {

      }

      public ReqMessage(string senderId, string recipientID)
      {
         Id = senderId;
         RecipientId = recipientID;
      }
   }
}
