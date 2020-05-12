using Blockcore.Platform.Networking.Messages;
using PubSub;
using System;
using System.Net;
using System.Net.Sockets;

namespace Blockcore.Platform.Networking.Handlers
{
    public class MessageMessageGatewayHandler : IMessageGatewayHandler, IHandle<MessageMessage>
    {
        private readonly Hub hub = Hub.Default;
        private readonly GatewayManager connectionManager;

        public MessageMessageGatewayHandler(GatewayManager connectionManager)
        {
            this.connectionManager = connectionManager;
        }

        public void Process(BaseMessage message, ProtocolType protocol, IPEndPoint endpoint = null, TcpClient client = null)
        {
            MessageMessage msg = (MessageMessage)message;
            Console.WriteLine("Message from {0}:{1}: {2}", endpoint.Address, endpoint.Port, msg.Content);
        }
    }
}
