using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blockcore.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Blockcore.Platform.Networking;
using Blockcore.Platform.Networking.Events;
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
using Blockcore.Platform.Networking.Messages;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Blockcore.Platform.Networking;
using Blockcore.Platform.Networking.Entities;
using Blockcore.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Blockcore.Platform.Networking.Entities;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Blockcore.Hub.Networking.Services;
using Blockcore.Platform;
using Blockcore.Platform.Networking;
using Blockcore.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Blockcore.Hub.Networking.Managers;
using Microsoft.AspNetCore.SignalR;
using Blockcore.Hub.Networking.Hubs;

namespace Blockcore.Hub.Networking.Managers
{
   //public class MessageModel
   //{
   //   public string Type { get; set; }

   //   public string Message { get; set; }

   //   public DateTime Date { get; set; }

   //   public string ClientUniqueId { get; set; }

   //   public dynamic Data { get; set; }
   //}

   public class HubManager : IDisposable
   {
      readonly int retryInterval = 10;
      private readonly ILogger<HubService> log;
      private readonly ChainSettings chainSettings;
      private readonly HubSettings hubSettings;

      private readonly PubSub.Hub hub = PubSub.Hub.Default;

      public IPEndPoint ServerEndpoint { get; private set; }

      public HubInfo LocalHubInfo { get; private set; }

      public List<Ack> AckResponces { get; private set; }

      private IPAddress internetAccessAdapter;
      private TcpClient TCPClient = new TcpClient();
      private readonly UdpClient UDPClient = new UdpClient();
      private Thread ThreadTCPListen;
      private Thread ThreadUDPListen;
      private bool _TCPListen = false;

      private readonly IHubContext<WebSocketHub> hubContext;

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

      private MessageSerializer messageSerializer;
      private IHubMessageProcessing messageProcessing;

      private readonly IServiceProvider serviceProvider;

      public ConnectionManager Connections { get; }

      public HubManager(
         ILogger<HubService> log,
         IOptions<ChainSettings> chainSettings,
         IOptions<HubSettings> hubSettings,
         IServiceProvider serviceProvider,
         IHubContext<WebSocketHub> hubContext,
         HubConnectionManager connectionManager)
      {
         this.log = log;
         this.chainSettings = chainSettings.Value;
         this.hubSettings = hubSettings.Value;
         this.hubContext = hubContext;
         this.serviceProvider = serviceProvider;

         Connections = connectionManager;
      }

      public Task StartAsync(CancellationToken cancellationToken)
      {
         log.LogInformation($"Start Hub Service for {chainSettings.Symbol}.");

         IPAddress[] IPAddresses = Dns.GetHostAddresses(hubSettings.Server);

         if (IPAddresses.Length == 0)
         {
            throw new ApplicationException("Did not find any IP address for the hub server.");
         }

         //ServerEndpoint = new IPEndPoint(IPAddress.Parse(hubSettings.Server), hubSettings.Port);
         // TODO: #4
         ServerEndpoint = new IPEndPoint(IPAddresses[0], hubSettings.Port);
         LocalHubInfo = new HubInfo();
         AckResponces = new List<Ack>();

         UDPClient.AllowNatTraversal(true);
         UDPClient.Client.SetIPProtectionLevel(IPProtectionLevel.Unrestricted);
         UDPClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

         LocalHubInfo.Name = Environment.MachineName;
         LocalHubInfo.ConnectionType = ConnectionTypes.Unknown;
         LocalHubInfo.Id = DateTime.Now.Ticks;

         IEnumerable<IPAddress> IPs = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork);

         foreach (IPAddress IP in IPs)
         {
            LocalHubInfo.InternalAddresses.Add(IP);
         }

         // To avoid circular reference we must resolve these in the startup.
         messageProcessing = serviceProvider.GetService<IHubMessageProcessing>();
         messageSerializer = serviceProvider.GetService<MessageSerializer>();

         // Prepare the messaging processors for message handling.
         MessageMaps maps = messageProcessing.Build();
         messageSerializer.Maps = maps;

         hub.Subscribe<ConnectionAddedEvent>(this, e =>
         {
            StringBuilder entry = new StringBuilder();

            entry.AppendLine($"ConnectionAddedEvent: {e.Data.Id}");
            entry.AppendLine($"                    : ExternalIPAddress: {e.Data.ExternalEndpoint}");
            entry.AppendLine($"                    : InternalIPAddress: {e.Data.InternalEndpoint}");
            entry.AppendLine($"                    : Name: {e.Data.Name}");

            foreach (System.Net.IPAddress address in e.Data.InternalAddresses)
            {
               entry.AppendLine($"                    : Address: {address}");
            }

            log.LogInformation(entry.ToString());

            //var msg = new MessageModel
            //{
            //   Type = "ConnectionAddedEvent",
            //   Date = DateTime.UtcNow,
            //   Message = entry.ToString(),
            //   Data = e
            //};

            hubContext.Clients.All.SendAsync("Event", e);
         });

         hub.Subscribe<ConnectionRemovedEvent>(this, e =>
         {
            log.LogInformation($"ConnectionRemovedEvent: {e.Data.Id}");

            //var msg = new MessageModel
            //{
            //   Type = "ConnectionRemovedEvent",
            //   Date = DateTime.UtcNow,
            //   Message = e.Data.ToString(),
            //   Data = e.Data
            //};

            hubContext.Clients.All.SendAsync("Event", e);
         });

         hub.Subscribe<ConnectionStartedEvent>(this, e =>
         {
            log.LogInformation($"ConnectionStartedEvent: {e.Endpoint}");

            //var msg = new MessageModel
            //{
            //   Type = "ConnectionStartedEvent",
            //   Date = DateTime.UtcNow,
            //   Message = e.Endpoint.ToString(),
            //   Data = e.Endpoint.ToString()
            //};

            hubContext.Clients.All.SendAsync("Event", e);
         });

         hub.Subscribe<ConnectionStartingEvent>(this, e =>
         {
            log.LogInformation($"ConnectionStartingEvent: {e.Data.Id}");

            //var msg = new MessageModel
            //{
            //   Type = "ConnectionStartingEvent",
            //   Date = DateTime.UtcNow,
            //   Message = e.Data.Id.ToString(),
            //   Data = e.Data.Id.ToString()
            //};

            hubContext.Clients.All.SendAsync("Event", e);
         });

         hub.Subscribe<ConnectionUpdatedEvent>(this, e =>
         {
            log.LogInformation($"ConnectionUpdatedEvent: {e.Data.Id}");

            //var msg = new MessageModel
            //{
            //   Type = "ConnectionUpdatedEvent",
            //   Date = DateTime.UtcNow,
            //   Message = e.Data.Id.ToString(),
            //   Data = e.Data.Id
            //};

            hubContext.Clients.All.SendAsync("Event", e);
         });

         hub.Subscribe<GatewayConnectedEvent>(this, e =>
         {
            log.LogInformation("Connected to Gateway");

            //var msg = new MessageModel
            //{
            //   Type = "GatewayConnectedEvent",
            //   Date = DateTime.UtcNow,
            //   Message = "",
            //   Data = ""
            //};

            hubContext.Clients.All.SendAsync("Event", e);
         });

         hub.Subscribe<GatewayShutdownEvent>(this, e =>
         {
            log.LogInformation("Disconnected from Gateway");

            //var msg = new MessageModel
            //{
            //   Type = "GatewayShutdownEvent",
            //   Date = DateTime.UtcNow,
            //   Message = "",
            //   Data = ""
            //};

            hubContext.Clients.All.SendAsync("Event", e);
         });

         hub.Subscribe<HubInfoEvent>(this, e =>
         {
            //var msg = new MessageModel
            //{
            //   Type = "HubInfoEvent",
            //   Date = DateTime.UtcNow,
            //   Message = e.Data.ToString(),
            //   Data = e.Data
            //};

            hubContext.Clients.All.SendAsync("Event", e);
         });

         hub.Subscribe<MessageReceivedEvent>(this, e =>
         {
            log.LogInformation($"MessageReceivedEvent: {e.Data.Content}");

            //var msg = new MessageModel
            //{
            //   Type = "MessageReceivedEvent",
            //   Date = DateTime.UtcNow,
            //   Message = e.Data.Content,
            //   Data = e.Data.Content
            //};

            hubContext.Clients.All.SendAsync("Event", e);
         });

         hub.Subscribe<GatewayErrorEvent>(this, e =>
         {
            log.LogInformation($"GatewayErrorEvent: {e.Message}");

            //var msg = new MessageModel
            //{
            //   Type = "GatewayErrorEvent",
            //   Date = DateTime.UtcNow,
            //   Message = e.Message,
            //   Data = e.Message
            //};

            hubContext.Clients.All.SendAsync("Event", e);
         });

         Task.Run(async () =>
         {
            try
            {
               bool connectedToGateway = false;

               while (!cancellationToken.IsCancellationRequested)
               {
                  //Task tcpTask = Task.Run(() =>
                  //{
                  //   TcpWorker(cancellationToken);
                  //}, cancellationToken);

                  //Task udTask = Task.Run(() =>
                  //{
                  //   UdpWorker(cancellationToken);
                  //}, cancellationToken);

                  //Task.WaitAll(new Task[] { tcpTask, udTask }, cancellationToken);

                  if (!connectedToGateway)
                  {
                     connectedToGateway = ConnectGateway();
                  }

                  // TODO: This loop will just continue to run after connected to gateway. It should check status and attempt to recycle and reconnect when needed.
                  Task.Delay(TimeSpan.FromSeconds(retryInterval), cancellationToken).Wait(cancellationToken);

                  //Task.Delay(TimeSpan.FromSeconds(retryInterval), cancellationToken).Wait(cancellationToken);

                  //var tokenSource = new CancellationTokenSource();
                  //cancellationToken.Register(() => { tokenSource.Cancel(); });

                  //try
                  //{
                  //   using (IServiceScope scope = scopeFactory.CreateScope())
                  //   {
                  //      Runner runner = scope.ServiceProvider.GetService<Runner>();
                  //      System.Collections.Generic.IEnumerable<Task> runningTasks = runner.RunAll(tokenSource);

                  //      Task.WaitAll(runningTasks.ToArray(), cancellationToken);

                  //      if (cancellationToken.IsCancellationRequested)
                  //      {
                  //         tokenSource.Cancel();
                  //      }
                  //   }

                  //   break;
                  //}
                  //catch (OperationCanceledException)
                  //{
                  //   // do nothing the task was cancel.
                  //   throw;
                  //}
                  //catch (AggregateException ae)
                  //{
                  //   if (ae.Flatten().InnerExceptions.OfType<SyncRestartException>().Any())
                  //   {
                  //      log.LogInformation("Sync: ### - Restart requested - ###");
                  //      log.LogTrace("Sync: Signalling token cancelation");
                  //      tokenSource.Cancel();

                  //      continue;
                  //   }

                  //   foreach (Exception innerException in ae.Flatten().InnerExceptions)
                  //   {
                  //      log.LogError(innerException, "Sync");
                  //   }

                  //   tokenSource.Cancel();

                  //   int retryInterval = 10;

                  //   log.LogWarning($"Unexpected error retry in {retryInterval} seconds");
                  //   //this.tracer.ReadLine();

                  //   // Blokcore Indexer is designed to be idempotent, we want to continue running even if errors are found.
                  //   // so if an unepxected error happened we log it wait and start again

                  //   Task.Delay(TimeSpan.FromSeconds(retryInterval), cancellationToken).Wait(cancellationToken);

                  //   continue;
                  //}
                  //catch (Exception ex)
                  //{
                  //   log.LogError(ex, "Sync");
                  //   break;
                  //}
               }
            }
            catch (OperationCanceledException)
            {
               // do nothing the task was cancel.
               throw;
            }
            catch (Exception ex)
            {
               log.LogError(ex, "Gateway");
               throw;
            }

         }, cancellationToken);

         return Task.CompletedTask;
      }

      public Task StopAsync(CancellationToken cancellationToken)
      {
         // Hm... we should disconnect our connection to both gateway and nodes, and inform then we are shutting down.
         // this.connectionManager.Disconnect
         // We will broadcast a shutdown when we're stopping.
         // connectionManager.BroadcastTCP(new Notification(NotificationsTypes.ServerShutdown, null));

         return Task.CompletedTask;
      }

      public bool ConnectGateway()
      {
         try
         {
            internetAccessAdapter = GetAdapterWithInternetAccess();

            log.LogInformation("Adapter with Internet Access: " + internetAccessAdapter);

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
            }))
            {
               IsBackground = true
            };

            keepAlive.Start();

            hub.Publish(new GatewayConnectedEvent());

            return true;
         }
         catch (Exception ex)
         {
            log.LogError($"Error when connecting to gateway {ServerEndpoint.ToString()}. Will retry again soon...", ex);
            hub.Publish(new GatewayErrorEvent($"Error when connecting to gateway {ServerEndpoint.ToString()}. Will retry again soon..."));
         }

         return false;
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

         foreach (ManagementBaseObject item in searcher.Get())
         {
            interfaceIndex = Convert.ToInt32(item["InterfaceIndex"]);
         }

         searcher = new ManagementObjectSearcher("root\\CIMV2", string.Format("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE InterfaceIndex={0}", interfaceIndex));

         foreach (ManagementBaseObject item in searcher.Get())
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
               log.LogError("Error on TCP send", ex);
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
            log.LogError("Error on UDP send", ex);
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
                        BaseMessage message = messageSerializer.Deserialize(receivedBytes);
                        messageProcessing.Process(message, ProtocolType.Udp, endpoint);
                     }
                  }
               }
               catch (Exception ex)
               {
                  log.LogError("Error on UDP Receive", ex);
               }
            }
         }))
         {
            IsBackground = true
         };

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
                  BaseMessage message = messageSerializer.Deserialize(TCPClient.GetStream());

                  //messageProcessing.Process(message, ProtocolType.Tcp, null, TCPClient);
                  messageProcessing.Process(message, ProtocolType.Tcp);
               }
               catch (Exception ex)
               {
                  log.LogError("Error on TCP Receive", ex);
               }
            }
         }))
         {
            IsBackground = true
         };

         if (TCPListen)
            ThreadTCPListen.Start();
      }

      public void ConnectToClient(HubInfo hubInfo)
      {
         Req req = new Req(LocalHubInfo.Id, hubInfo.Id);

         SendMessageTCP(req);

         log.LogInformation("Sent Connection Request To: " + hubInfo.ToString());

         Thread connect = new Thread(new ThreadStart(delegate
         {
            IPEndPoint responsiveEndpoint = FindReachableEndpoint(hubInfo);

            if (responsiveEndpoint != null)
            {
               log.LogInformation("Connection Successfull to: " + responsiveEndpoint.ToString());

               hub.Publish(new ConnectionStartedEvent() { Data = hubInfo, Endpoint = responsiveEndpoint });
            }
         }))
         {
            IsBackground = true
         };

         connect.Start();
      }

      public IPEndPoint FindReachableEndpoint(HubInfo hubInfo)
      {
         log.LogInformation("Attempting to Connect via LAN");

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

               log.LogInformation("Sending Ack to " + endpoint.ToString() + ". Attempt " + i + " of 3");

               SendMessageUDP(new Ack(LocalHubInfo.Id), endpoint);
               Thread.Sleep(200);

               Ack response = AckResponces.FirstOrDefault(a => a.RecipientId == hubInfo.Id);

               if (response != null)
               {
                  log.LogInformation("Received Ack Responce from " + endpoint.ToString());

                  hubInfo.ConnectionType = ConnectionTypes.LAN;

                  AckResponces.Remove(response);

                  return endpoint;
               }
            }
         }

         if (hubInfo.ExternalEndpoint != null)
         {
            log.LogInformation("Attempting to Connect via Internet");

            for (int i = 1; i < 100; i++)
            {
               if (!TCPClient.Connected)
               {
                  break;
               }

               log.LogInformation("Sending Ack to " + hubInfo.ExternalEndpoint + ". Attempt " + i + " of 99");

               SendMessageUDP(new Ack(LocalHubInfo.Id), hubInfo.ExternalEndpoint);
               Thread.Sleep(300);

               Ack response = AckResponces.FirstOrDefault(a => a.RecipientId == hubInfo.Id);

               if (response != null)
               {
                  log.LogInformation("Received Ack New from " + hubInfo.ExternalEndpoint.ToString());

                  hubInfo.ConnectionType = ConnectionTypes.WAN;

                  AckResponces.Remove(response);

                  return hubInfo.ExternalEndpoint;
               }
            }

            log.LogInformation("Connection to " + hubInfo.Name + " failed");
         }
         else
         {
            log.LogInformation("Client's External EndPoint is Unknown");
         }

         return null;
      }

      public void Dispose()
      {

      }
   }
}
