using Blockcore.Hub.Networking.Managers;
using Blockcore.Hub.Networking.Messages;
using Blockcore.Hub.Networking.Services;
using Blockcore.Platform.Networking.Entities;
using Blockcore.Platform.Networking.Messages;
using Microsoft.Extensions.Logging;
using PubSub;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Blockcore.Platform.Networking.Handlers.GatewayHandlers
{
   public class BroadcastMessageGatewayHandler : IGatewayMessageHandler, IHandle<BroadcastMessage>
   {
      private readonly PubSub.Hub hub = PubSub.Hub.Default;
      private readonly GatewayManager connectionManager;
      private readonly ILogger<MessageMessageGatewayHandler> log;

      public BroadcastMessageGatewayHandler(GatewayManager connectionManager, ILogger<MessageMessageGatewayHandler> log)
      {
         this.connectionManager = connectionManager;
         this.log = log;
      }

      public void Process(BaseMessage message, ProtocolType protocol, IPEndPoint endpoint = null, TcpClient client = null)
      {
         BroadcastMessage msg = (BroadcastMessage)message;

         if (msg.To == "HUB")
         {
            List<Entities.HubInfo> connections = connectionManager.Connections.GetBroadcastConnections(false);

            foreach (Entities.HubInfo connection in connections)
            {
               // Make sure we don't broadcast back to source.
               //if (connection.Id == msg.From)
               //{
               //   continue;
               //}

               // Should we only send TCP, and not UDP? Should we check if either is available?
               connectionManager.SendTCP(new Broadcast(msg), connection.Client);
            }
         }
         else if (msg.To == "GATEWAY")
         {
            Console.WriteLine($"Gateway receive a message from {msg.From} with content {msg.Content}.");
         }
         else
         {
            Console.WriteLine($"Gateway receive a message without a correct \"To\" value. Must be \"HUB\" or \"GATEWAY\".");
         }
      }
   }
}
