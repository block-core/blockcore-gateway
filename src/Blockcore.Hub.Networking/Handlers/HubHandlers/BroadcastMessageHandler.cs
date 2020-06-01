using Blockcore.Hub.Networking.Managers;
using Blockcore.Hub.Networking.Messages;
using Blockcore.Hub.Networking.Services;
using Blockcore.Platform.Networking.Entities;
using Blockcore.Platform.Networking.Events;
using Blockcore.Platform.Networking.Messages;
using PubSub;
using System;
using System.Net;
using System.Net.Sockets;

namespace Blockcore.Platform.Networking.Handlers.HubHandlers
{
   public class BroadcastMessageHandler : IHubMessageHandler, IHandle<BroadcastMessage>
   {
      private readonly PubSub.Hub hub = PubSub.Hub.Default;
      private readonly HubManager connectionManager;
      private readonly HubConnectionManager connections;

      public BroadcastMessageHandler(HubManager connectionManager, HubConnectionManager connections)
      {
         this.connectionManager = connectionManager;
         this.connections = connections;
   }

      public void Process(BaseMessage message, ProtocolType protocol, IPEndPoint endpoint = null, TcpClient client = null)
      {
         BroadcastMessage msg = (BroadcastMessage)message;
         Console.WriteLine($"HUB RECEIVED BROADCAST MESSAGE!! FROM: {msg.From} CONTENT: {msg.Content}.");

         // Publish the event on the local event hub.
         //hub.Publish(new MessageReceivedEvent() { From = msg.From, To = msg.To, Content = msg.Content });
         //connectionManager.GetConnection(msg.Id);

         // TODO: Debug and figure out what to do here.
         //if (msg.Id == "" || msg.Id == Guid.Empty.ToString())
         //{
         //   hub.Publish(new MessageReceivedEvent() { Data = new Message(msg) });
         //   //OnResultsUpdate.Invoke(this, msg.From + ": " + msg.Content);
         //} else if (endpoint != null & client != null)
         //{
         //   hub.Publish(new MessageReceivedEvent() { Data = new Message(msg) });
         //   //OnMessageReceived.Invoke(EP, new MessageReceivedEventArgs(CI, new Message(m), EP));
         //}
      }
   }
}
