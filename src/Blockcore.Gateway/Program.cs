using System.IO;
using System.Reflection;
using Blockcore.Hub.Networking.Managers;
using Blockcore.Hub.Networking.Services;
using Blockcore.Platform;
using Blockcore.Platform.Networking;
using Blockcore.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Blockcore.Gateway
{
   public class Program
   {
      public static void Main(string[] args)
      {
         CreateHostBuilder(args).Build().Run();
      }

      public static IHostBuilder CreateHostBuilder(string[] args) =>

          Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configHost =>
                {
                   configHost.SetBasePath(Directory.GetCurrentDirectory());
                   configHost.AddJsonFile("appsettings.json", optional: true);
                   configHost.AddEnvironmentVariables();
                   configHost.AddCommandLine(args);
                })
               .ConfigureAppConfiguration(config =>
               {
                  config.AddBlockcore("Blockore Gateway", args);
               })
              .ConfigureServices((hostContext, services) =>
              {
                 // services.Configure<GatewaySettings>(Configuration.GetSection("Gateway"));
                 services.Configure<GatewaySettings>(hostContext.Configuration.GetSection("Gateway"));

                 services.AddTransient<MessageSerializer>();
                 Assembly assembly = typeof(MessageSerializer).Assembly;

                 services.AddSingleton<IGatewayMessageProcessing, GatewayMessageProcessing>();
                 services.AddSingleton<GatewayConnectionManager>();
                 services.AddSingleton<GatewayManager>();
                 services.AddHostedService<GatewayService>();

                 // TODO: This should likely be updated in the future to allow third-party plugin assemblies to be loaded as well.
                 // Register all gateway handlers in executing assembly.
                 assembly.GetTypesImplementing<IGatewayMessageHandler>().ForEach((t) =>
                 {
                    services.AddSingleton(typeof(IGatewayMessageHandler), t);
                 });
              });
   }
}
