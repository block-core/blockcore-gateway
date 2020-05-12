using Blockcore.Platform.Networking.Messages;
using System.Net;
using System.Net.Sockets;

namespace Blockcore.Platform.Networking.Handlers.HubHandlers
{
   public class TestMessageHandler : IHubMessageHandler, IHandle<TestMessage>
   {
      public TestMessageHandler()
      {

      }

      public void Process(BaseMessage message, ProtocolType protocol, IPEndPoint endpoint = null, TcpClient client = null)
      {

      }
   }
}
