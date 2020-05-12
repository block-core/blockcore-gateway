using Blockcore.Hub.Networking.Managers;
using Blockcore.Hub.Networking.Services;
using Blockcore.Platform.Networking.Messages;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;

namespace Blockcore.Platform.Networking.Handlers.GatewayHandlers
{
   public class KeepAliveMessageGatewayHandler : IGatewayMessageHandler, IHandle<KeepAliveMessage>
   {
      private readonly ILogger<InfoMessageGatewayHandler> log;
      private readonly GatewayManager manager;

      public KeepAliveMessageGatewayHandler(ILogger<InfoMessageGatewayHandler> log, GatewayManager manager)
      {
         this.log = log;
         this.manager = manager;
      }

      public void Process(BaseMessage message, ProtocolType protocol, IPEndPoint endpoint = null, TcpClient client = null)
      {
         // This doesn't do anything, but nodes will send Keep Alive.
      }
   }
}
