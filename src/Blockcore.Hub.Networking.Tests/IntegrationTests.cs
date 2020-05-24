using Blockcore.Platform.Networking;
using Blockcore.Platform.Networking.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;
using Blockcore.Platform.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Blockcore.Hub;
using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using Blockcore.Platform.Networking.Entities;
using Blockcore.Hub.Networking;
using Blockcore.Hub.Networking.Managers;
using Blockcore.Hub.Networking.Extensions;

namespace Blockcore.Platform.Tests
{
   public static class Extensions
   {
      public static T GetService<T>(this IHost host)
      {
         return host.Services.GetService<T>();
      }
   }

   public class IntegrationTests
   {
      // private IHost orchestratorHost;
      private readonly ITestOutputHelper output;

      public IntegrationTests(ITestOutputHelper output)
      {
         this.output = output;

         // There will be only one orchestrator, so we'll create it here.
         //orchestratorHost = CreateOrchestratorHost();
      }

      private IHost CreateHubHost()
      {
         string[] args = new string[] { };
         IHostBuilder builder = Blockcore.Hub.Program.CreateHostBuilder(args);

         builder.ConfigureServices((hostContext, services) => {
            services.Remove<ILoggerFactory>();
            services.AddSingleton<ILoggerFactory, TestLoggerFactory>((ctx) => {
               return new TestLoggerFactory(output);
            });
         });

         IHost hub = builder.Build();

         return hub;
      }

      [Fact]
      public void HubIntegrationTest()
      {
         IHost host = CreateHubHost();
         HubManager hub = host.Services.GetService<HubManager>();

         hub.Setup(new Identity("obtain design mistake life call inside smooth gloom sunset bless winter tenant", 0));

      }

      //private IHost CreateHubHost()
      //{
      //   var args = new string[] { "config=hub1.json" };
      //   var builder = Hub.Program.CreateHostBuilder(args);

      //   builder.ConfigureServices(((hostContext, services) =>
      //   {
      //      services.Remove<ILoggerFactory>();
      //      services.AddSingleton<ILoggerFactory, TestLoggerFactory>((ctx) =>
      //      {
      //         return new TestLoggerFactory(this.output);
      //      });

      //      services.Remove<IHubManager>();
      //      services.AddSingleton<IHubManager, FakeHubManager>();

      //      // This is normally not registered in the Hub, only in the Orchestrator.
      //      //services.AddSingleton<IOrchestratorManager, FakeOrchestratorManager>();
      //   }));

      //   var hub = builder.Build();

      //   var hubManager = (FakeHubManager)hub.GetService<IHubManager>();
      //   var orchestratorManager = (FakeOrchestratorManager)orchestratorHost.GetService<IOrchestratorManager>();
      //   hubManager.SetOrchestrator(orchestratorManager);
      //   orchestratorManager.RegisterHubManager(hubManager);

      //   return hub;
      //}

      //private IHost CreateOrchestratorHost()
      //{
      //   var args = new string[] { "config=orchestrator1.json" };
      //   var builder = Orchestrator.Program.CreateHostBuilder(args);

      //   builder.ConfigureServices(((hostContext, services) =>
      //   {
      //      services.Remove<ILoggerFactory>();
      //      services.AddSingleton<ILoggerFactory, TestLoggerFactory>((ctx) =>
      //      {
      //         return new TestLoggerFactory(this.output);
      //      });

      //      services.Remove<IOrchestratorManager>();
      //      services.AddSingleton<IOrchestratorManager, FakeOrchestratorManager>();
      //   }));

      //   var orchestrator = builder.Build();

      //   return orchestrator;
      //}

      [Fact]
      public void GetSerializerAndVerifySerialization()
      {
         IHost host = CreateHubHost();
         MessageSerializer serializer = host.GetService<MessageSerializer>();
         byte[] bytes = serializer.Serialize(new AckMessage("1234"));
         Assert.NotEmpty(bytes);
      }

      [Fact]
      public void HubIntegrationTest2()
      {
         // Run setup on the OrchestratorHost so it can process  messages.
         //var orchestrator = orchestratorHost.GetService<OrchestratorHost>();
         //orchestrator.Setup();

         //var host1 = CreateHubHost();
         //var hub1 = host1.GetService<HubHost>();
         //var identity = new Identity("obtain design mistake life call inside smooth gloom sunset bless winter tenant");

         //// Configure all events and everything for this hub.
         //hub1.Setup(identity, CancellationToken.None);

         //var host2 = CreateHubHost();
         //var hub2 = host2.GetService<HubHost>();
         //Assert.NotEqual(hub1, hub2);

         //hub2.Setup(identity.GetIdentity(1), CancellationToken.None);

         //Assert.Equal("4b9715afa903c82b383abb74b6cd746bdd3beea3", hub1.Identity.Id);
         //Assert.Equal("309f0653f0bbc77c699b8920f263d65122c32740", hub2.Identity.Id);

         //// Generate a hub with random identity.
         //var host3 = CreateHubHost();
         //var hub3 = host3.GetService<HubHost>();
         //hub3.Setup(new Identity(), CancellationToken.None);

         //// Make hub1 trust hub2.
         //hub1.TrustedHubs.Add(hub2.Identity.Id);

         //// Now we must make our hubs connect to the orchestrator. Since we won't be doing any TCP/UDP communication,
         //// we must fake the messages forwarded over the wire. The port is used to identity individual hubs in the orchestrator,
         //// so they must be unique in the integration test.
         //hub1.Connect("127.0.0.1:6611");
         //hub2.Connect("127.0.0.1:6612");
         //hub3.Connect("127.0.0.1:6613");

         //// Verified that each of the hubs knows about 2 other hubs.
         //Assert.Equal(2, hub1.AvailableHubs.Count);
         //Assert.Equal(2, hub2.AvailableHubs.Count);
         //Assert.Equal(2, hub3.AvailableHubs.Count);

         //// Connect from hub2 to hub1, this should go automatic due to existing trust.
         //hub2.ConnectToHub(hub1.Identity.Id);

         //hub2.SendMessageToHub(new Message("Joe", "Sara", "Hi there! How are you?", hub1.Identity.Id), hub1.Identity.Id);
         ////hub1.SendMessageToHub(new Message("Sara", "Joe", "Hi again! I'm just fine!"), hub2.Identity.Id);
      }

      [Fact]
      public void InitialHubFlowTest()
      {
         // In this scenario, the 3 hubs will connect. One of the hubs already trust another hub from previous exchange.
         // The others will send requests to connect, which will be "manually" (automatic in the test) approved by the
         // hub owner.

         //var manager = new HubManager();

         //var hub1 = new Hub(0); // Identity 0
         //var hub2 = new Hub(1); // Identity 1
         //var hub3 = new Hub("obtain design mistake life call inside smooth gloom sunset bless winter tenant"); // Identity 0
         //var hub4 = new Hub("obtain design mistake life call inside smooth gloom sunset bless winter tenant", 1); // Identity 1

         //Assert.Equal("4b9715afa903c82b383abb74b6cd746bdd3beea3", hub3.Identity);
         //Assert.Equal("309f0653f0bbc77c699b8920f263d65122c32740", hub4.Identity);

         //// Ensure that we are on different identities for the same seed.
         //Assert.Equal(0, hub3.IdentityNumber);
         //Assert.Equal(1, hub4.IdentityNumber);

         //// Hub1 already trust Hub3.
         //hub1.TrustedHubs.Add(hub3.Identity);

         //Orchestrator orchestrator = new Orchestrator();
         //orchestrator.Register(new HubInfo(hub1));
         //orchestrator.Register(new HubInfo(hub2));
         //orchestrator.Register(new HubInfo(hub3));
         //orchestrator.Register(new HubInfo(hub4));

         //Assert.Equal(3, hub1.Hubs.Count);
         //Assert.Equal(3, hub2.Hubs.Count);
         //Assert.Equal(3, hub3.Hubs.Count);
         //Assert.Equal(3, hub4.Hubs.Count);

         //// Perform an update broadcast for hub3, and verify that the count does not go up.
         //orchestrator.Update(new HubInfo(hub3));
         //Assert.Equal(3, hub1.Hubs.Count);

         //// Disconnect the hub2 and verify counts.
         //orchestrator.Unregister(hub2.Identity);
         //Assert.Equal(2, hub1.Hubs.Count);

         //// Register the hub2 again, but use the update directly as oppose to register first.
         //orchestrator.Update(new HubInfo(hub2));
         //Assert.Equal(3, hub1.Hubs.Count);

         //// Initiate a connection between hub1 and hub2. This will result in an HubConnectRequest.
         //hub1.Connect(hub2.Identity);

         //// Initiate a connection between hub1 and hub3. This will result in an HubConnect.
         //hub1.Connect(hub3.Identity);

         //// Unregister all the hubs and validate that we've cleaned it up.
         //orchestrator.Unregister(hub1.Identity);
         //orchestrator.Unregister(hub2.Identity);
         //orchestrator.Unregister(hub3.Identity);
         //orchestrator.Unregister(hub4.Identity);

         //Assert.Empty(orchestrator.Hubs);
      }
   }
}
