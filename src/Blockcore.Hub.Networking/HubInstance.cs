using System;
using System.Collections.Generic;
using System.Text;

namespace Blockcore.Hub.Networking
{
   /// <summary>
   /// Represents an hub, can be either local or external. All known hub instances are retrieved to memory on the hub.
   /// </summary>
   public class HubInstance
   {
      public HubInstance(string publicKey) {

      }

      public DateTime Created { get; internal set; }
   }
}
