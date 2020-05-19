using Blockcore.Platform.Networking.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Blockcore.Platform.Networking
{
   public class HubConnectionManager : ConnectionManager
   { }

   public class GatewayConnectionManager : ConnectionManager
   { }

   public abstract class ConnectionManager
   {
      private List<HubInfo> Connections { get; } = new List<HubInfo>();

      public void RemoveConnection(HubInfo connection)
      {
         lock (Connections)
         {
            Connections.Remove(connection);
         }
      }

      public HubInfo GetConnection(string id)
      {
         return Connections.FirstOrDefault(x => x.Id == id);
      }

      public HubInfo GetConnection(TcpClient client)
      {
         return Connections.FirstOrDefault(x => x.Client == client);
      }

      public List<HubInfo> GetBroadcastConnections(bool isTcp)
      {
         if (isTcp)
         {
            return Connections.Where(x => x.Client != null).ToList();
         }
         else
         {
            return Connections;
         }
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
