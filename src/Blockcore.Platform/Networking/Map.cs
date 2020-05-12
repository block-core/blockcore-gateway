using System;
using System.Collections.Generic;

namespace Blockcore.Platform.Networking
{
    public class Map
    {
        public Map()
        {
            Handlers = new List<IMessageHandler>();
        }

        public ushort Command { get; set; }

        public Type MessageType { get; set; }

        public List<IMessageHandler> Handlers { get; }
    }
}
