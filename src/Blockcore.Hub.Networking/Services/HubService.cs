using System;
using System.Threading;
using System.Threading.Tasks;
using Blockcore.Hub.Networking.Managers;
using Blockcore.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Blockcore.Hub.Networking.Services
{
   public class HubService : IHostedService, IDisposable
   {
      private readonly HubManager manager;

      private readonly HubSettings settings;

      public HubService(HubManager manager, IOptions<HubSettings> settings)
      {
         this.manager = manager;
         this.settings = settings.Value;
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
