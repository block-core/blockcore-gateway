using Blockcore.Platform.Networking.Entities;
using Blockcore.Platform.Networking.Messages;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Sockets;

namespace Blockcore.Platform.Networking.Handlers
{
    public class InfoMessageGatewayHandler : IMessageGatewayHandler, IHandle<HubInfoMessage>
    {
        private readonly ILogger<InfoMessageGatewayHandler> log;
        private readonly GatewayManager manager;

        public InfoMessageGatewayHandler(ILogger<InfoMessageGatewayHandler> log, GatewayManager manager)
        {
            this.log = log;
            this.manager = manager;
        }

        public void Process(BaseMessage message, ProtocolType protocol, IPEndPoint endpoint = null, TcpClient client = null)
        {
            HubInfo hubInfo = manager.Connections.GetConnection(message.Id);

            if (hubInfo == null)
            {
                hubInfo = new HubInfo((HubInfoMessage)message);
                manager.Connections.AddConnection(hubInfo);

                if (endpoint != null)
                    this.log.LogInformation("Client Added: UDP EP: {0}:{1}, Name: {2}", endpoint.Address, endpoint.Port, hubInfo.Name);
                else if (client != null)
                    Console.WriteLine("Client Added: TCP EP: {0}:{1}, Name: {2}", ((IPEndPoint)client.Client.RemoteEndPoint).Address, ((IPEndPoint)client.Client.RemoteEndPoint).Port, hubInfo.Name);
            }
            else
            {
                hubInfo.Update((HubInfoMessage)message);

                if (endpoint != null)
                    this.log.LogInformation("Client Updated: UDP EP: {0}:{1}, Name: {2}", endpoint.Address, endpoint.Port, hubInfo.Name);
                else if (client != null)
                    this.log.LogInformation("Client Updated: TCP EP: {0}:{1}, Name: {2}", ((IPEndPoint)client.Client.RemoteEndPoint).Address, ((IPEndPoint)client.Client.RemoteEndPoint).Port, hubInfo.Name);
            }

            if (endpoint != null)
                hubInfo.ExternalEndpoint = endpoint;

            if (client != null)
                hubInfo.Client = client;

            manager.BroadcastTCP(hubInfo);

            if (!hubInfo.Initialized)
            {
                if (hubInfo.ExternalEndpoint != null & protocol == ProtocolType.Udp)
                    manager.SendUDP(new Message("Server", hubInfo.Name, "UDP Communication Test"), hubInfo.ExternalEndpoint);

                if (hubInfo.Client != null & protocol == ProtocolType.Tcp)
                    manager.SendTCP(new Message("Server", hubInfo.Name, "TCP Communication Test"), hubInfo.Client);

                if (hubInfo.Client != null & hubInfo.ExternalEndpoint != null)
                {
                    foreach (HubInfo ci in manager.Connections.Connections)
                    {
                        manager.SendUDP(ci, hubInfo.ExternalEndpoint);
                    }

                    hubInfo.Initialized = true;
                }
            }
        }
    }
}
