using Blockcore.Platform.Networking.Messages;
using System.Net;
using System.Net.Sockets;

namespace Blockcore.Platform.Networking
{
    public interface IMessageProcessingBase
    {
        public void Build();

        public void Process(BaseMessage message, ProtocolType protocol, IPEndPoint endpoint = null, TcpClient client = null);
    }
}
