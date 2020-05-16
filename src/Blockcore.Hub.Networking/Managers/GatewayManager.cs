using System;
using System.Collections.Generic;
using System.Text;
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

namespace Blockcore.Hub.Networking.Managers
{
   public class GatewayManager : IDisposable
   {
      private readonly ILogger<GatewayService> log;
      private readonly ChainSettings chainSettings;
      private readonly GatewaySettings gatewaySettings;
      private IPEndPoint tcpEndpoint;
      private TcpListener tcp;
      private UdpClient udp;
      private readonly int retryInterval = 10;
      private IGatewayMessageProcessing messageProcessing;
      private MessageSerializer messageSerializer;

      public IPEndPoint udpEndpoint;

      public ConnectionManager Connections { get; }

      private readonly IServiceProvider serviceProvider;

      public GatewayManager(
         ILogger<GatewayService> log,
         IOptions<ChainSettings> chainSettings,
         IOptions<GatewaySettings> gatewaySettings,
         IServiceProvider serviceProvider,
         GatewayConnectionManager connectionManager)
      {
         this.log = log;
         this.chainSettings = chainSettings.Value;
         this.gatewaySettings = gatewaySettings.Value;
         this.serviceProvider = serviceProvider;

         Connections = connectionManager;
      }

      public Task StartAsync(CancellationToken cancellationToken)
      {
         log.LogInformation($"Start Gateway Service for {chainSettings.Symbol}.");

         tcpEndpoint = new IPEndPoint(IPAddress.Any, gatewaySettings.Port);
         tcp = new TcpListener(tcpEndpoint);

         udpEndpoint = new IPEndPoint(IPAddress.Any, gatewaySettings.Port);
         udp = new UdpClient(udpEndpoint);

         // To avoid circular reference we must resolve these in the startup.
         messageProcessing = serviceProvider.GetService<IGatewayMessageProcessing>();
         messageSerializer = serviceProvider.GetService<MessageSerializer>();

         // Prepare the messaging processors for message handling.
         MessageMaps maps = messageProcessing.Build();
         messageSerializer.Maps = maps;

         Task.Run(async () =>
         {
            try
            {
               while (!cancellationToken.IsCancellationRequested)
               {
                  Task tcpTask = Task.Run(() =>
                  {
                     TcpWorker(cancellationToken);
                  }, cancellationToken);

                  Task udTask = Task.Run(() =>
                  {
                     UdpWorker(cancellationToken);
                  }, cancellationToken);

                  Task.WaitAll(new Task[] { tcpTask, udTask }, cancellationToken);

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

         log.LogInformation($"TCP listener started on port {gatewaySettings.Port}.");
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

      public Task StopAsync(CancellationToken cancellationToken)
      {
         // We will broadcast a shutdown when we're stopping.
         BroadcastTCP(new Notification(NotificationsTypes.ServerShutdown, null));

         return Task.CompletedTask;
      }

      private void TcpWorker(CancellationToken token)
      {
         StartTcp();

         while (!token.IsCancellationRequested)
         {
            try
            {
               TcpClient newClient = Tcp.AcceptTcpClient();

               Action<object> processData = new Action<object>(delegate (object tcp)
               {
                  TcpClient client = (TcpClient)tcp;
                  client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

                  while (client.Connected)
                  {
                     try
                     {
                        // Retrieve the message from the network stream. This will handle everything from message headers, body and type parsing.
                        Platform.Networking.Messages.BaseMessage message = messageSerializer.Deserialize(client.GetStream());

                        messageProcessing.Process(message, ProtocolType.Tcp, null, client);
                     }
                     catch (Exception ex)
                     {
                        log.LogError(ex, "Failed to process incoming message.");
                        Disconnect(client);
                     }
                  }

                  Disconnect(client);
               });

               Thread threadProcessData = new Thread(new ParameterizedThreadStart(processData));
               threadProcessData.Start(newClient);
            }
            catch (Exception ex)
            {
               log.LogError(ex, "TCP error");

               // We'll sleep a short while before connecting, to avoid extensive resource usage.
               Thread.Sleep(250);
            }
         }
      }

      private void UdpWorker(CancellationToken token)
      {
         log.LogInformation($"UDP listener started on port {udpEndpoint.Port}.");

         while (!token.IsCancellationRequested)
         {
            try
            {
               byte[] receivedBytes = Udp.Receive(ref udpEndpoint);

               if (receivedBytes != null)
               {
                  // Retrieve the message from the network stream. This will handle everything from message headers, body and type parsing.
                  Platform.Networking.Messages.BaseMessage message = messageSerializer.Deserialize(receivedBytes);
                  messageProcessing.Process(message, ProtocolType.Udp, udpEndpoint);
               }
            }
            catch (Exception ex)
            {
               log.LogError(ex, "UDP error");
            }
         }
      }

      public void Dispose()
      {

      }
   }
}
