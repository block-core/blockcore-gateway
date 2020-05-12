using Blockcore.Platform.Networking.Exceptions;
using System;
using System.Collections.Generic;

namespace Blockcore.Platform.Networking
{
    public class MessageMaps
    {
        private readonly Dictionary<ushort, Map> maps;

        public MessageMaps()
        {
            maps = new Dictionary<ushort, Map>();
        }

        public bool Contains(ushort command)
        {
            return maps.ContainsKey(command);
        }

        public void AddCommand(ushort command, Map map)
        {
            maps.Add(command, map);
        }

        public void AddHandler(ushort command, IMessageHandler handler)
        {
            maps[command].Handlers.Add(handler);
        }

        public Map GetMap(ushort command)
        {
            return maps[command];
        }

        public Type GetMessageType(ushort command)
        {
            if (!maps.ContainsKey(command))
            {
                throw new MessageProcessingException($"There exists no registered messages types for the command {command}.");
            }

            var map = maps[command];
            return map.MessageType;
        }
    }
}
