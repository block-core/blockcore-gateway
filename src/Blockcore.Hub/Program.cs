using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Blockcore.Hub
{
   public class Program
   {
      public static void Main(string[] args)
      {
         CreateHostBuilder(args).Build().Run();
      }

      public static IHostBuilder CreateHostBuilder(string[] args) =>
          Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(config =>
            {
               config.AddBlockcore("Blockore Hub", args);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
               webBuilder.ConfigureKestrel(serverOptions =>
               {
                  serverOptions.AddServerHeader = false;
               });

               webBuilder.UseStartup<Startup>();
            });
   }
}
