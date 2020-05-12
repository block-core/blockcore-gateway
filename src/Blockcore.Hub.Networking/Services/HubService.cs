using System;
using System.Threading;
using System.Threading.Tasks;
using Blockcore.Hub.Networking.Managers;
using Microsoft.Extensions.Hosting;

namespace Blockcore.Hub.Networking.Services
{
   public class HubService : IHostedService, IDisposable
   {
      private readonly HubManager manager;

      public HubService(HubManager manager)
      {
         this.manager = manager;
      }

      public Task StartAsync(CancellationToken cancellationToken)
      {
         return manager.StartAsync(cancellationToken);
      }

      public Task StopAsync(CancellationToken cancellationToken)
      {
         return manager.StopAsync(cancellationToken);
      }
      public void Dispose()
      {

      }
   }
}
