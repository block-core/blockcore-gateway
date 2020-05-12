using Blockcore.Platform.Networking.Entities;
using Blockcore.Platform.Networking.Events;
using Microsoft.Extensions.Logging;
using PubSub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Blockcore.Platform.Networking
{
    public class HubManager
    {
        public IPEndPoint ServerEndpoint { get; }

        public HubInfo LocalHubInfo { get; }

        public List<Ack> AckResponces { get; }

        private IPAddress internetAccessAdapter;
        private TcpClient TCPClient = new TcpClient();
        private UdpClient UDPClient = new UdpClient();
        private Thread ThreadTCPListen;
        private Thread ThreadUDPListen;
        private bool _TCPListen = false;

        public bool TCPListen
        {
            get { return _TCPListen; }
            set
            {
                _TCPListen = value;
                if (value)
                    ListenTCP();
            }
        }

        private bool _UDPListen = false;
        public bool UDPListen
        {
            get { return _UDPListen; }
            set
            {
                _UDPListen = value;
                if (value)
                    ListenUDP();
            }
        }

        private readonly ILogger<HubManager> log;
        private readonly MessageSerializer messageSerializer;
        private readonly Hub hub = Hub.Default;

        public ConnectionManager Connections { get; }

        public IMessageProcessingBase MessageProcessing { get; set; }

        public HubManager(
            ILogger<HubManager> log,
            ConnectionManager connectionManager,
            MessageSerializer messageSerializer)
        {
            this.log = log;
            this.messageSerializer = messageSerializer;
            this.Connections = connectionManager;
            this.ServerEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5060);
            this.LocalHubInfo = new HubInfo();
            this.AckResponces = new List<Ack>();

            UDPClient.AllowNatTraversal(true);
            UDPClient.Client.SetIPProtectionLevel(IPProtectionLevel.Unrestricted);
            UDPClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            LocalHubInfo.Name = Environment.MachineName;
            LocalHubInfo.ConnectionType = ConnectionTypes.Unknown;
            LocalHubInfo.Id = DateTime.Now.Ticks;

            var IPs = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            foreach (var IP in IPs)
            {
                LocalHubInfo.InternalAddresses.Add(IP);
            }
        }

        public void ConnectGateway()
        {
            try
            {
                internetAccessAdapter = GetAdapterWithInternetAccess();

                this.log.LogInformation("Adapter with Internet Access: " + internetAccessAdapter);

                TCPClient = new TcpClient();
                TCPClient.Client.Connect(ServerEndpoint);

                UDPListen = true;
                TCPListen = true;

                SendMessageUDP(LocalHubInfo.Simplified(), ServerEndpoint);
                LocalHubInfo.InternalEndpoint = (IPEndPoint)UDPClient.Client.LocalEndPoint;

                Thread.Sleep(550);
                SendMessageTCP(LocalHubInfo);

                Thread keepAlive = new Thread(new ThreadStart(delegate
                {
                    while (TCPClient.Connected)
                    {
                        Thread.Sleep(5000);
                        SendMessageTCP(new KeepAlive());
                    }
                }));

                keepAlive.IsBackground = true;
                keepAlive.Start();

                hub.Publish(new GatewayConnectedEvent());

            }
            catch (Exception ex)
            {
                this.log.LogError("Error when connecting", ex);
            }
        }

        public void DisconnectedGateway()
        {
            TCPClient.Client.Disconnect(true);

            UDPListen = false;
            TCPListen = false;

            Connections.ClearConnections();
        }

        public void ConnectOrDisconnect()
        {
            if (TCPClient.Connected)
            {
                DisconnectedGateway();
            }
            else
            {
                ConnectGateway();
            }
        }

        private IPAddress GetAdapterWithInternetAccess()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_IP4RouteTable WHERE Destination=\"0.0.0.0\"");
            
            int interfaceIndex = -1;

            foreach (var item in searcher.Get())
            { 
                interfaceIndex = Convert.ToInt32(item["InterfaceIndex"]);
            }

            searcher = new ManagementObjectSearcher("root\\CIMV2", string.Format("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE InterfaceIndex={0}", interfaceIndex));

            foreach (var item in searcher.Get())
            {
                string[] IPAddresses = (string[])item["IPAddress"];

                foreach (string IP in IPAddresses)
                { 
                    return IPAddress.Parse(IP);
                }
            }

            return null;
        }

        public void SendMessageTCP(IBaseEntity entity)
        {
            if (TCPClient != null && TCPClient.Connected)
            {
                byte[] data = messageSerializer.Serialize(entity.ToMessage());

                try
                {
                    NetworkStream NetStream = TCPClient.GetStream();
                    NetStream.Write(data, 0, data.Length);
                }
                catch (Exception ex)
                {
                    this.log.LogError("Error on TCP send", ex);
                }
            }
        }

        public void SendMessageUDP(IBaseEntity entity, IPEndPoint endpoint)
        {
            entity.Id = LocalHubInfo.Id;

            byte[] data = messageSerializer.Serialize(entity.ToMessage());

            try
            {
                if (data != null)
                { 
                    UDPClient.Send(data, data.Length, endpoint);
                }
            }
            catch (Exception ex)
            {
                this.log.LogError("Error on UDP send", ex);
            }
        }

        private void ListenUDP()
        {
            ThreadUDPListen = new Thread(new ThreadStart(delegate
            {
                while (UDPListen)
                {
                    try
                    {
                        IPEndPoint endpoint = LocalHubInfo.InternalEndpoint;

                        if (endpoint != null)
                        {
                            byte[] receivedBytes = UDPClient.Receive(ref endpoint);

                            if (receivedBytes != null)
                            {
                                // Retrieve the message from the network stream. This will handle everything from message headers, body and type parsing.
                                var message = messageSerializer.Deserialize(receivedBytes);
                                MessageProcessing.Process(message, ProtocolType.Udp, endpoint);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.log.LogError("Error on UDP Receive", ex);
                    }
                }
            }));

            ThreadUDPListen.IsBackground = true;

            if (UDPListen)
            { 
                ThreadUDPListen.Start();
            }
        }

        private void ListenTCP()
        {
            ThreadTCPListen = new Thread(new ThreadStart(delegate
            {
                while (TCPListen)
                {
                    try
                    {
                        // Retrieve the message from the network stream. This will handle everything from message headers, body and type parsing.
                        var message = messageSerializer.Deserialize(TCPClient.GetStream());

                        //messageProcessing.Process(message, ProtocolType.Tcp, null, TCPClient);
                        MessageProcessing.Process(message, ProtocolType.Tcp);
                    }
                    catch (Exception ex)
                    {
                        this.log.LogError("Error on TCP Receive", ex);
                    }
                }
            }));

            ThreadTCPListen.IsBackground = true;

            if (TCPListen)
                ThreadTCPListen.Start();
        }

        public void ConnectToClient(HubInfo hubInfo)
        {
            Req req = new Req(LocalHubInfo.Id, hubInfo.Id);

            SendMessageTCP(req);

            this.log.LogInformation("Sent Connection Request To: " + hubInfo.ToString());

            Thread connect = new Thread(new ThreadStart(delegate
            {
                IPEndPoint responsiveEndpoint = FindReachableEndpoint(hubInfo);

                if (responsiveEndpoint != null)
                {
                    this.log.LogInformation("Connection Successfull to: " + responsiveEndpoint.ToString());

                    hub.Publish(new ConnectionStartedEvent() { Data = hubInfo, Endpoint = responsiveEndpoint });
                }
            }));

            connect.IsBackground = true;

            connect.Start();
        }

        public IPEndPoint FindReachableEndpoint(HubInfo hubInfo)
        {
            this.log.LogInformation("Attempting to Connect via LAN");

            for (int ip = 0; ip < hubInfo.InternalAddresses.Count; ip++)
            {
                if (!TCPClient.Connected)
                { 
                    break;
                }

                IPAddress IP = hubInfo.InternalAddresses[ip];
                IPEndPoint endpoint = new IPEndPoint(IP, hubInfo.InternalEndpoint.Port);

                for (int i = 1; i < 4; i++)
                {
                    if (!TCPClient.Connected)
                    { 
                        break;
                    }

                    this.log.LogInformation("Sending Ack to " + endpoint.ToString() + ". Attempt " + i + " of 3");

                    SendMessageUDP(new Ack(LocalHubInfo.Id), endpoint);
                    Thread.Sleep(200);

                    Ack response = AckResponces.FirstOrDefault(a => a.RecipientId == hubInfo.Id);

                    if (response != null)
                    {
                        this.log.LogInformation("Received Ack Responce from " + endpoint.ToString());

                        hubInfo.ConnectionType = ConnectionTypes.LAN;

                        AckResponces.Remove(response);

                        return endpoint;
                    }
                }
            }

            if (hubInfo.ExternalEndpoint != null)
            {
                this.log.LogInformation("Attempting to Connect via Internet");

                for (int i = 1; i < 100; i++)
                {
                    if (!TCPClient.Connected)
                    { 
                        break;
                    }

                    this.log.LogInformation("Sending Ack to " + hubInfo.ExternalEndpoint + ". Attempt " + i + " of 99");

                    SendMessageUDP(new Ack(LocalHubInfo.Id), hubInfo.ExternalEndpoint);
                    Thread.Sleep(300);

                    Ack response = AckResponces.FirstOrDefault(a => a.RecipientId == hubInfo.Id);

                    if (response != null)
                    {
                        this.log.LogInformation("Received Ack New from " + hubInfo.ExternalEndpoint.ToString());

                        hubInfo.ConnectionType = ConnectionTypes.WAN;

                        AckResponces.Remove(response);

                        return hubInfo.ExternalEndpoint;
                    }
                }

                this.log.LogInformation("Connection to " + hubInfo.Name + " failed");
            }
            else
            {
                this.log.LogInformation("Client's External EndPoint is Unknown");
            }

            return null;
        }
    }
}
