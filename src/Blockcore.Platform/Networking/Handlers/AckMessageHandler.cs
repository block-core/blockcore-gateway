using Blockcore.Platform.Networking.Entities;
using Blockcore.Platform.Networking.Events;
using Blockcore.Platform.Networking.Messages;
using Microsoft.Extensions.Logging;
using PubSub;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Blockcore.Platform.Networking.Handlers
{
    public class AckMessageHandler : IMessageHandler, IHandle<AckMessage>
    {
        private readonly Hub hub = Hub.Default;
        private readonly ILogger<AckMessageHandler> log;
        private readonly HubManager manager;

        public AckMessageHandler(ILogger<AckMessageHandler> log, HubManager manager)
        {
            this.log = log;
            this.manager = manager;
        }

        public void Process(BaseMessage message, ProtocolType Protocol, IPEndPoint EP = null, TcpClient Client = null)
        {
            AckMessage msg = (AckMessage)message;

            if (msg.Response)
            {
                manager.AckResponces.Add(new Ack(msg));
            }
            else
            {
                var CI = manager.Connections.GetConnection(msg.Id);

                if (CI.ExternalEndpoint.Address.Equals(EP.Address) & CI.ExternalEndpoint.Port != EP.Port)
                {
                    this.log.LogInformation("Received Ack on Different Port (" + EP.Port + "). Updating ...");

                    CI.ExternalEndpoint.Port = EP.Port;

                    hub.Publish(new ConnectionUpdatedEvent() { Data = CI });
                }

                List<string> IPs = new List<string>();
                CI.InternalAddresses.ForEach(new Action<IPAddress>(delegate (IPAddress IP) { IPs.Add(IP.ToString()); }));

                if (!CI.ExternalEndpoint.Address.Equals(EP.Address) & !IPs.Contains(EP.Address.ToString()))
                {
                    this.log.LogInformation("Received Ack on New Address (" + EP.Address + "). Updating ...");

                    CI.InternalAddresses.Add(EP.Address);
                }

                msg.Response = true;
                msg.RecipientId = manager.LocalHubInfo.Id;
                manager.SendMessageUDP(new Ack(msg), EP);
            }
        }
    }
}
