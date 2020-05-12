using Blockcore.Platform.Networking.Messages;
using System.Net;
using System.Net.Sockets;

namespace Blockcore.Platform.Networking.Handlers.HubHandlers
{
   public class KeepAliveMessageHandler : IHubMessageHandler, IHandle<KeepAliveMessage>
   {
      public void Process(BaseMessage message, ProtocolType protocol, IPEndPoint endpoint = null, TcpClient client = null)
      {
         KeepAliveMessage msg = (KeepAliveMessage)message;
      }
   }
}
