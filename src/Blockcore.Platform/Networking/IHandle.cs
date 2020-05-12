using Blockcore.Platform.Networking.Messages;
using System.Net;
using System.Net.Sockets;

namespace Blockcore.Platform.Networking
{
    public interface IHandle<T> where T : BaseMessage
    {
        void Process(BaseMessage message, ProtocolType protocol, IPEndPoint endpoint = null, TcpClient client = null);
    }
}
