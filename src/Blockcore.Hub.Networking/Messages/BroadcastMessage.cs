using System;
using System.Collections.Generic;
using System.Text;
using Blockcore.Platform.Networking.Messages;
using MessagePack;

namespace Blockcore.Hub.Networking.Messages
{
   /// <summary>
   /// This message is sent from source hub and relayed through the gateway to all connected hubs. The source hub does not receive the broadcast.
   /// </summary>
   [MessagePackObject]
   public class BroadcastMessage : BaseMessage
   {
      public override ushort Command => MessageTypes.BROADCAST;

      [Key(1)]
      public string From { get; set; }

      /// <summary>
      /// Should be "HUB" or "GATEWAY", never anything else.
      /// </summary>
      [Key(2)]
      public string To { get; set; }

      [Key(3)]
      public string Content { get; set; }
   }
}
