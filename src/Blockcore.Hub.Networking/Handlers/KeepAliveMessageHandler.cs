using Blockcore.Platform.Networking.Messages;
using System.Net;
using System.Net.Sockets;

namespace Blockcore.Platform.Networking.Handlers
{
    public class KeepAliveMessageHandler : IMessageHandler, IHandle<KeepAliveMessage>
    {
        public void Process(BaseMessage message, ProtocolType protocol, IPEndPoint endpoint = null, TcpClient client = null)
        {
            KeepAliveMessage msg = (KeepAliveMessage)message;
        }
    }
}
