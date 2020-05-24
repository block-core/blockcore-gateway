using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Blockcore.Platform.Tests
{
   public class NetworkTests
   {
      private readonly ITestOutputHelper output;

      public NetworkTests(ITestOutputHelper output)
      {
         this.output = output;
      }

      [Fact]
      public async Task ConnectUpHubs()
      {
         //_ = Task.Run(async () =>
         //{
         //   var args = new string[] { "config=gateway1.json" };
         //   Gateway.Program.CreateHostBuilder(args).Build().Run();
         //});

         //_ = Task.Run(async () =>
         //{
         //   await Task.Delay(2000);
         //   var args = new string[] { "config=hub1.json" };
         //   Hub.Program.CreateHostBuilder(args).Build().Run();
         //});

         //_ = Task.Run(async () =>
         //{
         //   await Task.Delay(1000);
         //   var args = new string[] { "config=hub2.json" };
         //   Hub.Program.CreateHostBuilder(args).Build().Run();
         //});

         //await Task.Delay(10000);
      }
   }
}
