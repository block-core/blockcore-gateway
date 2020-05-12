using Blockcore.Platform.Networking.Entities;
using System.Net;

namespace Blockcore.Platform.Networking.Events
{
    public class ConnectionStartedEvent
    {
        public HubInfo Data { get; set; }

        public IPEndPoint Endpoint { get; set; }
    }
}
