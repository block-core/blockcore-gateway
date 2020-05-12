using Blockcore.Hub.Networking.Managers;
using Blockcore.Hub.Networking.Services;
using Blockcore.Platform.Networking.Events;
using Blockcore.Platform.Networking.Messages;
using PubSub;
using System.Net;
using System.Net.Sockets;

namespace Blockcore.Platform.Networking.Handlers.HubHandlers
{
   public class NotificationMessageHandler : IHubMessageHandler, IHandle<NotificationMessage>
   {
      private readonly PubSub.Hub hub = PubSub.Hub.Default;
      private readonly HubManager manager;
      private readonly HubConnectionManager connections;

      public NotificationMessageHandler(HubManager manager, HubConnectionManager connections)
      {
         this.manager = manager;
         this.connections = connections;
      }

      public void Process(BaseMessage message, ProtocolType Protocol, IPEndPoint EP = null, TcpClient Client = null)
      {
         NotificationMessage item = (NotificationMessage)message;

         if (item.Type == NotificationsTypes.Disconnected)
         {
            Entities.HubInfo hubInfo = connections.GetConnection(long.Parse(item.Tag.ToString()));

            if (hubInfo != null)
            {
               connections.RemoveConnection(hubInfo);
               hub.Publish(new ConnectionRemovedEvent() { Data = hubInfo });
            }
         }
         else if (item.Type == NotificationsTypes.ServerShutdown)
         {
            manager.DisconnectedGateway();
            hub.Publish(new GatewayShutdownEvent());
         }
      }
   }
}
