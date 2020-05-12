using Blockcore.Platform.Networking.Messages;
using System.Net;
using System.Net.Sockets;

namespace Blockcore.Platform.Networking.Handlers
{
    public class TestMessageHandler : IMessageHandler, IHandle<TestMessage>
    {
        public TestMessageHandler()
        {

        }

        public void Process(BaseMessage message, ProtocolType protocol, IPEndPoint endpoint = null, TcpClient client = null)
        {
            
        }
    }
}
