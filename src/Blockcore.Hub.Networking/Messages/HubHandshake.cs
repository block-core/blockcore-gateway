using System;
using System.Collections.Generic;
using System.Text;

namespace Blockcore.Hub.Networking.Messages
{
   /// <summary>
   /// Hub to hub handshake message that acts as an request to connect together.
   /// </summary>
    public class HubHandshake
    {
      public string Message { get; set; }
    }
}
