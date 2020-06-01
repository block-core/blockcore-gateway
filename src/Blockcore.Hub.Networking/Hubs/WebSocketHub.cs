using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Blockcore.Hub.Networking.Managers;
using Blockcore.Platform.Networking.Messages;
using System.Security.Cryptography.X509Certificates;
using Blockcore.Platform.Networking.Entities;
using Microsoft.AspNetCore.Mvc;
using Blockcore.Hub.Networking.Services;
using NBitcoin;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.VisualBasic;
using Blockcore.Hub.Networking.Messages;

namespace Blockcore.Hub.Networking.Hubs
{
   public class MessageModel
   {
      // public string PublicKey { get; set; }

      public string From { get; set; }

      public string Content { get; set; }

      //public DateTime Date { get; set; }

      //public string Type { get; set; }
   }

   public class WebSocketHub : Microsoft.AspNetCore.SignalR.Hub
   {
      private readonly ILogger<WebSocketHub> log;
      private readonly CommandDispatcher commandDispatcher;
      private readonly HubManager hubManager;
      private readonly AvailableServices availableServices;

      public WebSocketHub(ILogger<WebSocketHub> log, CommandDispatcher commandDispatcher, HubManager hubManager, AvailableServices availableServices)
      {
         this.log = log;
         this.commandDispatcher = commandDispatcher;
         this.hubManager = hubManager;
         this.availableServices = availableServices;
      }

      public void ConnectToPeer(string peerId)
      {
         hubManager.ConnectToClient(peerId);

         // Send message to connect us to a specified hub.
         //hubManager.SendMessageTCP(new ReqMessage(hubManager.LocalHubInfo.Id, peerId));
      }

      public AvailableServices AvailableServices()
      {
         return availableServices;
         //Clients.Caller.SendAsync("AvailableServices", availableServices);
      }

      public void DisconnectToPeer(string peerId)
      {
         hubManager.DisconnectToClient(peerId);

         // Send message to connect us to a specified hub.
         //hubManager.SendMessageTCP(new ReqMessage(hubManager.LocalHubInfo.Id, peerId));
      }

      /// <summary>
      /// Sends a broadcast message to directly connected hubs.
      /// </summary>
      public void BroadcastToHubs(MessageModel message)
      {
         List<HubInfo> allConnections = hubManager.Connections.GetBroadcastConnections(false);

         foreach (HubInfo conn in allConnections)
         {
            // Skip self.
            if (conn.Id == hubManager.LocalHubInfo.Id)
            {
               continue;
            }

            var msg = new Message { From = message.From, To = conn.Id, Content = message.Content };
            msg.From = hubManager.LocalHubInfo.Id;
            msg.To = conn.Id;
            msg.RecipientId = conn.Id;

            // Send the message only directly to the Hub.
            //hubManager.SendMessageTCP(msg, conn.ExternalEndpoint);
         }
      }

      /// <summary>
      /// Sends a broadcast message to all hubs connected to the gateways.
      /// </summary>
      public void BroadcastToHubsRelayed(MessageModel message)
      {
         hubManager.SendMessageTCP(new Broadcast { To = "HUB", From = hubManager.LocalHubInfo.Id, Command = MessageTypes.BROADCAST, Id = hubManager.LocalHubInfo.Id, Content = message.Content });
      }

      /// <summary>
      /// Sends a broadcast message to connected gateways.
      /// </summary>
      public void BroadcastToGateways(MessageModel message)
      {
         hubManager.SendMessageTCP(new Broadcast { To = "GATEWAY", From = hubManager.LocalHubInfo.Id, Command = MessageTypes.BROADCAST, Id = hubManager.LocalHubInfo.Id, Content = message.Content });
      }

      ///// <summary>
      ///// Basic echo method that can be used to verify connection.
      ///// </summary>
      ///// <param name="message">Any message to echo back.</param>
      ///// <returns>Returns the same message supplied.</returns>
      //public void Broadcast(MessageModel message)
      //{
      //   List<HubInfo> allConnections = hubManager.Connections.GetBroadcastConnections(false);

      //   foreach (HubInfo conn in allConnections)
      //   {
      //      // Skip self.
      //      if (conn.Id == hubManager.LocalHubInfo.Id)
      //      {
      //         continue;
      //      }

      //      var msg = new Message { From = message.PublicKey, To = "Everyone", Content = message.Message };
      //      msg.From = hubManager.LocalHubInfo.Id;
      //      msg.To = conn.Id;
      //      msg.RecipientId = conn.Id;

      //      hubManager.SendMessageTCP(msg);
      //   }

      //   //return Clients.Caller.SendAsync("Message", message);
      //}

      //public Task Command(string type, string command, object[]? args)
      //{
      //   string result = commandDispatcher.Execute(type, command, args);
      //   return Clients.Caller.SendAsync("Command", result);
      //}
   }
}
