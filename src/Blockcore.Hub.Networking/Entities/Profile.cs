using System;
using System.Collections.Generic;
using System.Text;

namespace Blockcore.Hub.Networking.Entities
{
   public class Profile
   {
      public string Id { get; set; }

      public string Name { get; set; }

      public string Shortname { get; set; }

      public string Alias { get; set; }

      public string Title { get; set; }

      public DateTime Timestamp { get; set; }
   }
}
