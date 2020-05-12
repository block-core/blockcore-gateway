using Blockcore.Platform.Networking.Entities;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Blockcore.Platform.Networking
{
    public class GatewayManager
    {
        private int port = 4366;
        private IPEndPoint tcpEndpoint;
        private TcpListener tcp;
        public IPEndPoint udpEndpoint;
        private UdpClient udp;
        private readonly ILogger<GatewayManager> log;
        private readonly MessageSerializer messageSerializer;

        public ConnectionManager Connections { get; }

        public GatewayManager(
            ILogger<GatewayManager> log,
            MessageSerializer messageSerializer,
            ConnectionManager connectionManager)
        {
            this.port = 5060;
            this.log = log;
            this.messageSerializer = messageSerializer;
            this.Connections = connectionManager;

            tcpEndpoint = new IPEndPoint(IPAddress.Any, port);
            tcp = new TcpListener(tcpEndpoint);

            udpEndpoint = new IPEndPoint(IPAddress.Any, port);
            udp = new UdpClient(udpEndpoint);
        }

        public void SendTCP(IBaseEntity entity, TcpClient client)
        {
            if (client != null && client.Connected)
            {
                byte[] Data = messageSerializer.Serialize(entity.ToMessage());

                NetworkStream NetStream = client.GetStream();
                NetStream.Write(Data, 0, Data.Length);
            }
        }

        public void SendUDP(IBaseEntity Item, IPEndPoint EP)
        {
            byte[] Bytes = messageSerializer.Serialize(Item.ToMessage());

            udp.Send(Bytes, Bytes.Length, udpEndpoint);
        }

        public void BroadcastTCP(IBaseEntity Item)
        {
            foreach (HubInfo CI in Connections.Connections.Where(x => x.Client != null))
            { 
                SendTCP(Item, CI.Client);
            }
        }

        public void BroadcastUDP(IBaseEntity Item)
        {
            foreach (HubInfo CI in Connections.Connections)
                SendUDP(Item, CI.ExternalEndpoint);
        }

        public TcpListener Tcp { get { return tcp; } }

        public UdpClient Udp { get { return udp; } }

        //public IPEndPoint UdpEndpoint { get { return udpEndpoint; } set { udpEndpoint = value; } }

        public void StartTcp()
        {
            tcp.Start();

            log.LogInformation($"TCP listener started on port {port}.");
        }

        public void StartUdp()
        {

        }

        public void Disconnect(TcpClient Client)
        {
            HubInfo CI = Connections.Connections.FirstOrDefault(x => x.Client == Client);

            if (CI != null)
            {
                Connections.RemoveConnection(CI);

                log.LogInformation($"Client disconnected {Client.Client.RemoteEndPoint}");

                Client.Close();

                BroadcastTCP(new Notification(NotificationsTypes.Disconnected, CI.Id));
            }
        }
    }
}
