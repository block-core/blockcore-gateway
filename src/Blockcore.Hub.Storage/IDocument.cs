using System;
using System.Collections.Generic;
using System.Text;

namespace Blockcore.Hub.Storage
{
   public class Document
   {
      public string Id { get; set; }

      public string Type { get; set; }

      public string Signature { get; set; }

      public string ContentType { get; set; }

      public byte[] Data { get; set; }
   }
}
