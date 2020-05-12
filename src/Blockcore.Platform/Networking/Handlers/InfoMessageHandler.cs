using Blockcore.Platform.Networking.Entities;
using Blockcore.Platform.Networking.Events;
using Blockcore.Platform.Networking.Messages;
using PubSub;
using System.Net;
using System.Net.Sockets;

namespace Blockcore.Platform.Networking.Handlers
{
    public class InfoMessageHandler : IMessageHandler, IHandle<HubInfoMessage>
    {
        private Hub hub = Hub.Default;
        private readonly HubManager manager;

        public InfoMessageHandler(HubManager manager)
        {
            this.manager = manager;
        }

        public void Process(BaseMessage message, ProtocolType Protocol, IPEndPoint EP = null, TcpClient Client = null)
        {
            var msg = (HubInfoMessage)message;

            // We need this lock as we're checking for null and during an initial handshake messages will come almost simultaneously.
            lock (manager.Connections)
            {
                HubInfo hubInfo = manager.Connections.GetConnection(msg.Id);

                if (hubInfo == null)
                {
                    hubInfo = new HubInfo(msg);
                    manager.Connections.AddConnection(hubInfo);

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
