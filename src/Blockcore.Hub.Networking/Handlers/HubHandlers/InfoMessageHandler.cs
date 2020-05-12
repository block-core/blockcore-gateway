using Blockcore.Hub.Networking.Managers;
using Blockcore.Hub.Networking.Services;
using Blockcore.Platform.Networking.Entities;
using Blockcore.Platform.Networking.Events;
using Blockcore.Platform.Networking.Messages;
using PubSub;
using System.Net;
using System.Net.Sockets;

namespace Blockcore.Platform.Networking.Handlers.HubHandlers
{
   public class InfoMessageHandler : IHubMessageHandler, IHandle<HubInfoMessage>
   {
      private readonly PubSub.Hub hub = PubSub.Hub.Default;
      private readonly HubManager manager;
      private readonly HubConnectionManager connections;

      public InfoMessageHandler(HubManager manager, HubConnectionManager connections)
      {
         this.manager = manager;
         this.connections = connections;
      }

      public void Process(BaseMessage message, ProtocolType Protocol, IPEndPoint EP = null, TcpClient Client = null)
      {
         var msg = (HubInfoMessage)message;

         // We need this lock as we're checking for null and during an initial handshake messages will come almost simultaneously.
         //lock (manager.Connections)
         lock (connections)
         {
            HubInfo hubInfo = connections.GetConnection(msg.Id);

            if (hubInfo == null)
            {
               hubInfo = new HubInfo(msg);
               connections.AddConnection(hubInfo);

               hub.Publish(new ConnectionAddedEvent() { Data = hubInfo });
            }
            else
            {
               hubInfo.Update(msg);

               hub.Publish(new ConnectionUpdatedEvent() { Data = hubInfo });
            }
         }
      }
   }
}
