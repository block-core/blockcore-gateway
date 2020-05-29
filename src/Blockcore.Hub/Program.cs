using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Blockcore.Hub
{
   public class Program
   {
      public static void Main(string[] args)
      {
         //Task.Run(() => {
         //   CreateHostBuilder(args).Build().Run();
         //});

         OpenBrowser("http://localhost:9912");

         CreateHostBuilder(args).Build().Run();

         //var window = new WebWindows.WebWindow("Blockcore");
         //window.NavigateToUrl("https://localhost:9912");
         ////window.NavigateToUrl("http://localhost:4200");
         //window.WaitForExit();
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

      public static void OpenBrowser(string url)
      {
         try
         {
            Process.Start(url);
         }
         catch
         {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
               url = url.Replace("&", "^&");
               Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
               Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
               Process.Start("open", url);
            }
            else
            {
               throw;
            }
         }
      }
   }
}
