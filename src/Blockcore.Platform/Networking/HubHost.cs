using Blockcore.Platform.Networking.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PubSub;
using System;
using System.Reflection;
using System.Threading;

namespace Blockcore.Platform.Networking
{
    public class HubHost
    {
        public static HubHost Start(string[] args)
        {
            //setup our DI
            var serviceProvider = new ServiceCollection();

            serviceProvider.AddSingleton<HubHost>();
            serviceProvider.AddSingleton<IMessageProcessingBase, MessageProcessing>();
            serviceProvider.AddSingleton<HubManager>();
            serviceProvider.AddSingleton<MessageSerializer>();
            serviceProvider.AddSingleton<MessageMaps>();
            serviceProvider.AddSingleton<ConnectionManager>();
            serviceProvider.AddLogging();

            // Register all handlers.
            Assembly.GetExecutingAssembly().GetTypesImplementing<IMessageHandler>().ForEach((t) =>
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
            var host = services.GetService<HubHost>();
            host.Launch(args);

            logger.LogInformation("All done!");

            return host;
        }

        private readonly ILogger<HubHost> log;
        private readonly IMessageProcessingBase messageProcessing;
        private readonly HubManager manager;
        private readonly Hub hub = Hub.Default;

        public HubHost(
            ILogger<HubHost> log,
            IMessageProcessingBase messageProcessing,
            HubManager manager)
        {
            this.log = log;
            this.messageProcessing = messageProcessing;
            this.manager = manager;

            // Due to circular dependency, we must manually set the MessageProcessing on the Manager.
            this.manager.MessageProcessing = this.messageProcessing;
        }

        public void Launch(string[] args)
        {
            // Prepare the messaging processors for message handling.
            this.messageProcessing.Build();

            manager.ConnectGateway();

            hub.Subscribe<ConnectionAddedEvent>(this, e =>
            {
                Console.WriteLine($"ConnectionAddedEvent: {e.Data.Id}");
                Console.WriteLine($"                    : ExternalIPAddress: {e.Data.ExternalEndpoint}");
                Console.WriteLine($"                    : InternalIPAddress: {e.Data.InternalEndpoint}");
                Console.WriteLine($"                    : Name: {e.Data.Name}");

                foreach (var address in e.Data.InternalAddresses)
                {
                    Console.WriteLine($"                    : Address: {address}");
                }
            });

            hub.Subscribe<ConnectionRemovedEvent>(this, e =>
            {
                Console.WriteLine($"ConnectionRemovedEvent: {e.Data.Id}");
            });

            hub.Subscribe<ConnectionStartedEvent>(this, e =>
            {
                Console.WriteLine($"ConnectionStartedEvent: {e.Endpoint}");
            });

            hub.Subscribe<ConnectionStartingEvent>(this, e =>
            {
                Console.WriteLine($"ConnectionStartedEvent: {e.Data.Id}");
            });

            hub.Subscribe<ConnectionUpdatedEvent>(this, e =>
            {
                Console.WriteLine($"ConnectionUpdatedEvent: {e.Data.Id}");
            });

            hub.Subscribe<GatewayConnectedEvent>(this, e =>
            {
                Console.WriteLine("Connected to Gateway");
            });

            hub.Subscribe<GatewayShutdownEvent>(this, e =>
            {
                Console.WriteLine("Disconnected from Gateway");
            });

            hub.Subscribe<HubInfoEvent>(this, e =>
            {
            });

            hub.Subscribe<MessageReceivedEvent>(this, e =>
            {
                Console.WriteLine($"MessageReceivedEvent: {e.Data.Content}");
            });
        }

        public void Stop()
        {
            // Hm... we should disconnect our connection to both gateway and nodes, and inform then we are shutting down.
            // this.connectionManager.Disconnect
            // We will broadcast a shutdown when we're stopping.
            // connectionManager.BroadcastTCP(new Notification(NotificationsTypes.ServerShutdown, null));
        }
    }
}
