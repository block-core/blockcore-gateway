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
   public class ReqMessageHandler : IHubMessageHandler, IHandle<ReqMessage>
   {
      private readonly PubSub.Hub hub = PubSub.Hub.Default;
      private readonly HubManager manager;
      private readonly HubConnectionManager connections;

      public ReqMessageHandler(HubManager manager, HubConnectionManager connections)
      {
         this.manager = manager;
         this.connections = connections;
      }

      public void Process(BaseMessage message, ProtocolType Protocol, IPEndPoint EP = null, TcpClient Client = null)
      {
         ReqMessage msg = (ReqMessage)message;
         HubInfo hubInfo = connections.GetConnection(msg.RecipientId);

         if (hubInfo != null)
         {
            hub.Publish(new ConnectionStartingEvent() { Data = hubInfo });

            IPEndPoint ResponsiveEP = manager.FindReachableEndpoint(hubInfo);

            if (ResponsiveEP != null)
            {
               hub.Publish(new ConnectionStartedEvent() { Data = hubInfo, Endpoint = ResponsiveEP });
               hub.Publish(new ConnectionUpdatedEvent() { Data = hubInfo });
            }
         }
      }
   }
}
