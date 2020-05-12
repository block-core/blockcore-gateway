using Blockcore.Platform.Networking.Entities;
using Blockcore.Platform.Networking.Events;
using Blockcore.Platform.Networking.Messages;
using PubSub;
using System.Net;
using System.Net.Sockets;

namespace Blockcore.Platform.Networking.Handlers
{
    public class ReqMessageHandler : IMessageHandler, IHandle<ReqMessage>
    {
        private readonly Hub hub = Hub.Default;
        private readonly HubManager manager;

        public ReqMessageHandler(HubManager manager)
        {
            this.manager = manager;
        }

        public void Process(BaseMessage message, ProtocolType Protocol, IPEndPoint EP = null, TcpClient Client = null)
        {
            ReqMessage msg = (ReqMessage)message;
            HubInfo hubInfo = manager.Connections.GetConnection(msg.RecipientId);

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
