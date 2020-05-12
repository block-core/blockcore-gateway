using System.Reflection;
using Blockcore.Hub.Networking.Managers;
using Blockcore.Hub.Networking.Services;
using Blockcore.Platform;
using Blockcore.Platform.Networking;
using Blockcore.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Blockcore.Hub
{
   public class Startup
   {
      public Startup(IConfiguration configuration)
      {
         Configuration = configuration;
      }

      public IConfiguration Configuration { get; }

      // This method gets called by the runtime. Use this method to add services to the container.
      public void ConfigureServices(IServiceCollection services)
      {
         services.Configure<ChainSettings>(Configuration.GetSection("Chain"));
         services.Configure<NetworkSettings>(Configuration.GetSection("Network"));
         services.Configure<IndexerSettings>(Configuration.GetSection("Indexer"));
         services.Configure<HubSettings>(Configuration.GetSection("Hub"));
         services.Configure<GatewaySettings>(Configuration.GetSection("Gateway"));

         services.AddSingleton<IHubMessageProcessing, HubMessageProcessing>();
         services.AddSingleton<IGatewayMessageProcessing, GatewayMessageProcessing>();

         services.AddTransient<MessageSerializer>();

         services.AddSingleton<HubConnectionManager>();
         services.AddSingleton<GatewayConnectionManager>();

         Assembly assembly = typeof(MessageSerializer).Assembly;

         // TODO: This should likely be updated in the future to allow third-party plugin assemblies to be loaded as well.
         // Register all gateway handlers in executing assembly.
         assembly.GetTypesImplementing<IGatewayMessageHandler>().ForEach((t) =>
         {
            services.AddSingleton(typeof(IGatewayMessageHandler), t);
         });

         // Register all hub handlers.
         assembly.GetTypesImplementing<IHubMessageHandler>().ForEach((t) =>
         {
            services.AddSingleton(typeof(IHubMessageHandler), t);
         });

         services.AddSingleton<GatewayManager>();
         services.AddSingleton<HubManager>();

         services.AddHostedService<GatewayService>();
         services.AddHostedService<HubService>();

         services.AddControllers();
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
      {
         if (env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
         }

         app.UseRouting();

         app.UseAuthorization();

         app.UseEndpoints(endpoints =>
         {
            endpoints.MapControllers();
         });
      }
   }
}
