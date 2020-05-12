using Blockcore.Platform.Networking.Messages;
using System.Net;
using System.Net.Sockets;

namespace Blockcore.Platform.Networking
{
   public interface IHubMessageProcessing
   {
      public MessageMaps Build();

      public void Process(BaseMessage message, ProtocolType protocol, IPEndPoint endpoint = null, TcpClient client = null);
   }

   public interface IGatewayMessageProcessing
   {
      public MessageMaps Build();

      public void Process(BaseMessage message, ProtocolType protocol, IPEndPoint endpoint = null, TcpClient client = null);
   }
}
