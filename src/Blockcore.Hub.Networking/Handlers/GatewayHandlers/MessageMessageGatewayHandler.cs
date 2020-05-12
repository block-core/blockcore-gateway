using Blockcore.Hub.Networking.Managers;
using Blockcore.Hub.Networking.Services;
using Blockcore.Platform.Networking.Messages;
using Microsoft.Extensions.Logging;
using PubSub;
using System;
using System.Net;
using System.Net.Sockets;

namespace Blockcore.Platform.Networking.Handlers.GatewayHandlers
{
   public class MessageMessageGatewayHandler : IGatewayMessageHandler, IHandle<MessageMessage>
   {
      private readonly PubSub.Hub hub = PubSub.Hub.Default;
      private readonly GatewayManager connectionManager;
      private readonly ILogger<MessageMessageGatewayHandler> log;

      public MessageMessageGatewayHandler(GatewayManager connectionManager, ILogger<MessageMessageGatewayHandler> log)
      {
         this.connectionManager = connectionManager;
         this.log = log;
      }

      public void Process(BaseMessage message, ProtocolType protocol, IPEndPoint endpoint = null, TcpClient client = null)
      {
         MessageMessage msg = (MessageMessage)message;
         log.LogInformation("Message from {0}:{1}: {2}", endpoint.Address, endpoint.Port, msg.Content);
      }
   }
}
