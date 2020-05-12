using Blockcore.Platform.Networking;
using System;
using System.Threading;

namespace Blockcore.Gateway
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Blockcore Gateway Starting...");
            var host = GatewayHost.Start(args);
            Console.WriteLine("Blockcore Gateway Started.");
            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();

            host.Stop();
            Thread.Sleep(3000);
        }
    }
}
