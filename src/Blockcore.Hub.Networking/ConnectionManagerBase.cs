using Blockcore.Platform.Networking.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Blockcore.Platform.Networking
{
   public class HubConnectionManager : ConnectionManager
   { }

   public class GatewayConnectionManager : ConnectionManager
   { }

   public abstract class ConnectionManager
   {
      // TODO: Refactor dependent types to not use this collection directly.
      public List<HubInfo> Connections { get; private set; } = new List<HubInfo>();

      public void RemoveConnection(HubInfo connection)
      {
         lock (Connections)
         {
            Connections.Remove(connection);
         }
      }

      public HubInfo GetConnection(long id)
      {
         return Connections.FirstOrDefault(x => x.Id == id);
      }

      public void AddConnection(HubInfo hubInfo)
      {
         lock (Connections)
         {
            Connections.Add(hubInfo);
         }
      }

      public void ClearConnections()
      {
         lock (Connections)
         {
            Connections.Clear();
         }
      }
   }
}
