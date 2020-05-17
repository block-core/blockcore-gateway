using System;
using System.Reflection;
using Blockcore.Hub.Networking.Hubs;
using Blockcore.Hub.Networking.Managers;
using Blockcore.Hub.Networking.Services;
using Blockcore.Platform;
using Blockcore.Platform.Networking;
using Blockcore.Settings;
using Blockcore.Utilities.JsonConverters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

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

         services.AddTransient<MessageSerializer>();
         Assembly assembly = typeof(MessageSerializer).Assembly;

         services.AddSingleton<IHubMessageProcessing, HubMessageProcessing>();
         services.AddSingleton<HubConnectionManager>();
         services.AddSingleton<HubManager>();
         services.AddSingleton<WebSocketHub>();
         services.AddSingleton<CommandDispatcher>();
         services.AddHostedService<HubService>();

         // Register all hub handlers.
         assembly.GetTypesImplementing<IHubMessageHandler>().ForEach((t) =>
         {
            services.AddSingleton(typeof(IHubMessageHandler), t);
         });

         services.AddResponseCompression();

         // services.AddControllers();

         services.AddControllersWithViews();
         // In production, the Angular files will be served from this directory
         services.AddSpaStaticFiles(configuration =>
         {
            configuration.RootPath = "ClientApp/dist";
         });

         services.AddSignalR().AddNewtonsoftJsonProtocol(options =>
         {
            var settings = new JsonSerializerSettings();
            Serializer.RegisterFrontConverters(settings);
            options.PayloadSerializerSettings = settings;
         });

         // Add service and create Policy to allow Cross-Origin Requests
         services.AddCors
         (
             options =>
             {
                options.AddPolicy
                   (
                       "CorsPolicy",

                       builder =>
                       {
                          string[] allowedDomains = new[] { "http://localhost", "http://localhost:9912", "http://localhost:4200", "http://localhost:8080" };

                          builder
                          .WithOrigins(allowedDomains)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                       }
                   );
             });
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
      {
         if (env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
         }

         app.UseResponseCompression();

         app.UseDefaultFiles();

         app.UseStaticFiles();

         if (!env.IsDevelopment())
         {
            app.UseSpaStaticFiles();
         }

         app.UseRouting();

         app.UseCors("CorsPolicy");

         app.UseEndpoints(endpoints =>
         {
            endpoints.MapHub<WebSocketHub>("/ws");
            endpoints.MapControllers();
         });

         app.UseSpa(spa =>
         {
            // To learn more about options for serving an Angular SPA from ASP.NET Core,
            // see https://go.microsoft.com/fwlink/?linkid=864501

            spa.Options.SourcePath = "ClientApp";
            // spa.Options.StartupTimeout = new TimeSpan(0, 5, 0);

            if (env.IsDevelopment())
            {
               spa.UseAngularCliServer(npmScript: "start");
            }
         });
      }
   }

   /// <summary>
   /// This class will allow to read the wwwroot folder
   /// which has been set ad an embeded folder in to the dll (in the project file)
   /// </summary>
   //public class EditorRCLConfigureOptions : IPostConfigureOptions<StaticFileOptions>
   //{
   //   private readonly IWebHostEnvironment environment;

   //   public EditorRCLConfigureOptions(IWebHostEnvironment environment)
   //   {
   //      this.environment = environment;
   //   }

   //   public void PostConfigure(string name, StaticFileOptions options)
   //   {
   //      // Basic initialization in case the options weren't initialized by any other component
   //      options.ContentTypeProvider = options.ContentTypeProvider ?? new FileExtensionContentTypeProvider();

   //      if (options.FileProvider == null && environment.WebRootFileProvider == null)
   //      {
   //         throw new InvalidOperationException("Missing FileProvider.");
   //      }

   //      options.FileProvider = options.FileProvider ?? environment.WebRootFileProvider;

   //      // Add our provider
   //      var filesProvider = new ManifestEmbeddedFileProvider(GetType().Assembly, "wwwroot");
   //      options.FileProvider = new CompositeFileProvider(options.FileProvider, filesProvider);
   //   }
   //}
}
