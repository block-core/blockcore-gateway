using Blockcore.Platform.Networking.Messages;

namespace Blockcore.Platform.Networking.Entities
{
   public class KeepAlive : BaseEntity
   {
      public KeepAlive(string id)
      {
         Id = id;
      }

      public override BaseMessage ToMessage()
      {
         return new KeepAliveMessage() { Id = Id };
      }
   }
}
