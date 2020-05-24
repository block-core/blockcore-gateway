using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blockcore.Hub.Storage
{
   public interface IHubStorage
   {
      Task StartAsync();

      void Save(Document document);

      Document Load(string type, string id);
   }
}
