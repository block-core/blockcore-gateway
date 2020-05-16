using Blockcore.Platform.Networking.Messages;

namespace Blockcore.Platform.Networking.Entities
{
   public class Message : BaseEntity
   {
      public string From { get; set; }
      public string To { get; set; }
      public string Content { get; set; }
      public long RecipientId { get; set; }

      public Message()
      {

      }

      public Message(string from, string to, string content)
      {
         From = from;
         To = to;
         Content = content;
      }

      public Message(MessageMessage message)
      {
         From = message.From;
         To = message.To;
         Content = message.Content;
         Id = message.Id;
         RecipientId = message.RecipientId;
      }

      public override BaseMessage ToMessage()
      {
         var msg = new MessageMessage();

         msg.From = From;
         msg.To = To;
         msg.Content = Content;
         msg.Id = Id;
         msg.RecipientId = RecipientId;

         return msg;
      }
   }
}
