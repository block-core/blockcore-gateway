using Blockcore.Platform.Networking.Events;
using Blockcore.Platform.Networking.Messages;
using PubSub;
using System.Net;
using System.Net.Sockets;

namespace Blockcore.Platform.Networking.Handlers
{
    public class NotificationMessageHandler : IMessageHandler, IHandle<NotificationMessage>
    {
        private readonly Hub hub = Hub.Default;
        private readonly HubManager manager;

        public NotificationMessageHandler(HubManager manager)
        {
            this.manager = manager;
        }

        public void Process(BaseMessage message, ProtocolType Protocol, IPEndPoint EP = null, TcpClient Client = null)
        {
            NotificationMessage item = (NotificationMessage)message;

            if (item.Type == NotificationsTypes.Disconnected)
            {
                var hubInfo = manager.Connections.GetConnection(long.Parse(item.Tag.ToString()));

                if (hubInfo != null)
                {
                    manager.Connections.RemoveConnection(hubInfo);
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
