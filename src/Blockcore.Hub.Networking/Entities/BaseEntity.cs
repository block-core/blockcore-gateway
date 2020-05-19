using Blockcore.Platform.Networking.Messages;

namespace Blockcore.Platform.Networking.Entities
{
   public abstract class BaseEntity : IBaseEntity
   {
      public string Id { get; set; }

      public virtual ushort Command { get; set; }

      public abstract BaseMessage ToMessage();
   }
}
