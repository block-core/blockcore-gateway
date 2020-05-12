using Blockcore.Platform.Networking;
using System;
using System.Threading;

namespace Blockcore.Hub
{
   class Program
   {
      static void Main(string[] args)
      {
         if (args[0] == "--gateway")
         {
            Console.WriteLine("Blockcore Gateway Starting...");

            var host = GatewayHost.Start(args);

            Console.WriteLine("Blockcore Gateway Started.");
            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();

            host.Stop();
            Thread.Sleep(3000);
         }
         else
         {
            Console.WriteLine("Blockcore Hub Starting...");

            var host = HubHost.Start(args);

            Console.WriteLine("Blockcore Hub Started.");
            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();

            host.Stop();
            Thread.Sleep(3000);
         }
      }
   }
}
