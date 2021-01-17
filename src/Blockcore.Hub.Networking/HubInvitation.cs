using System;
using System.Collections.Generic;
using System.Text;

namespace Blockcore.Hub.Networking
{
   public enum InvitationDirection
   {
      Incoming,
      Outgoing
   }

   /// <summary>
   /// 
   /// </summary>
   public class HubInvitation
   {
      public DateTime Created { get; set; }

      public InvitationDirection Direction { get; set; }

      public string HubId { get; set; }
   }
}
