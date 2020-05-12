using Blockcore.Platform.Networking.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Blockcore.Platform.Networking
{
    public class GatewayHost
    {
        // TODO: Consider changing to using the host model/framework and IService. For now, this will suffice.
        public static GatewayHost Start(string[] args)
        {
            //setup our DI
            var serviceProvider = new ServiceCollection();

            serviceProvider.AddSingleton<GatewayHost>();
            serviceProvider.AddSingleton<IMessageProcessingBase, MessageProcessing>();
            serviceProvider.AddSingleton<GatewayManager>();
            serviceProvider.AddSingleton<MessageSerializer>();
            serviceProvider.AddSingleton<MessageMaps>();
            serviceProvider.AddSingleton<ConnectionManager>();
            serviceProvider.AddLogging();

            // TODO: This should likely be updated in the future to allow third-party plugin assemblies to be loaded as well.
            // Register all handlers in executing assembly.
            Assembly.GetExecutingAssembly().GetTypesImplementing<IMessageGatewayHandler>().ForEach((t) =>
            {
                serviceProvider.AddSingleton(typeof(IMessageHandler), t);
            });

            var services = serviceProvider.BuildServiceProvider();

            //configure console logging
            services.GetService<ILoggerFactory>();

            var logger = services.GetService<ILoggerFactory>().CreateLogger<GatewayHost>();
            logger.LogInformation("Starting application");

            CancellationTokenSource token = new CancellationTokenSource();

            //do the actual work here
            var host = services.GetService<GatewayHost>();
            host.Launch(args);

            logger.LogInformation("All done!");

            return host;
        }

        private readonly ILogger<GatewayHost> log;
        private readonly IMessageProcessingBase messageProcessing;
        private readonly MessageSerializer messageSerializer;
        private readonly GatewayManager connectionManager;

        public GatewayHost(
            ILogger<GatewayHost> log,
            IMessageProcessingBase messageProcessing,
            MessageSerializer messageSerializer,
            GatewayManager connectionManager)
        {
            this.log = log;
            this.messageProcessing = messageProcessing;
            this.messageSerializer = messageSerializer;
            this.connectionManager = connectionManager;
        }

        public void Launch(string[] args)
        {
            // Prepare the messaging processors for message handling.
            this.messageProcessing.Build();

            var token = CancellationToken.None;

            Task tcpTask = Task.Run(() => {
                TcpWorker(token);
            }, token);

            Task udTask = Task.Run(() => {
                UdpWorker(token);
            }, token);
        }

        public void Stop()
        {
            // We will broadcast a shutdown when we're stopping.
            connectionManager.BroadcastTCP(new Notification(NotificationsTypes.ServerShutdown, null));
        }

        private void TcpWorker(CancellationToken token)
        {
            connectionManager.StartTcp();

            while (!token.IsCancellationRequested)
            {
                try
                {
                    TcpClient newClient = connectionManager.Tcp.AcceptTcpClient();

                    Action<object> processData = new Action<object>(delegate (object tcp)
                    {
                        TcpClient client = (TcpClient)tcp;
                        client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

                        while (client.Connected)
                        {
                            try
                            {
                                // Retrieve the message from the network stream. This will handle everything from message headers, body and type parsing.
                                var message = messageSerializer.Deserialize(client.GetStream());
                                
                                messageProcessing.Process(message, ProtocolType.Tcp, null, client);
                            }
                            catch (Exception ex)
                            {
                                this.log.LogError(ex, "Failed to process incoming message.");
                                connectionManager.Disconnect(client);
                            }
                        }

                        connectionManager.Disconnect(client);
                    });

                    Thread threadProcessData = new Thread(new ParameterizedThreadStart(processData));
                    threadProcessData.Start(newClient);
                }
                catch (Exception ex)
                {
                    this.log.LogError(ex, "TCP error");

                    // We'll sleep a short while before connecting, to avoid extensive resource usage.
                    Thread.Sleep(250);
                }
            }
        }

        private void UdpWorker(CancellationToken token)
        {
            log.LogInformation($"UDP listener started on port {connectionManager.udpEndpoint.Port}.");

            while (!token.IsCancellationRequested)
            {
                try
                {
                    byte[] receivedBytes = connectionManager.Udp.Receive(ref connectionManager.udpEndpoint);

                    if (receivedBytes != null)
                    {
                        // Retrieve the message from the network stream. This will handle everything from message headers, body and type parsing.
                        var message = messageSerializer.Deserialize(receivedBytes);
                        messageProcessing.Process(message, ProtocolType.Udp, connectionManager.udpEndpoint);
                    }
                }
                catch (Exception ex)
                {
                    log.LogError(ex, "UDP error");
                }
            }
        }
    }
}
