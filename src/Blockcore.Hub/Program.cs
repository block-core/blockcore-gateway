using Blockcore.Platform.Networking;
using System;
using System.Threading;

namespace Blockcore.Hub
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Blockcore Hub Starting...");
            var host = HubHost.Start(args);
            Console.WriteLine("Blockcore Hub Started.");
            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();

            host.Stop();
            Thread.Sleep(3000);
        }
    }
}
