using Blockcore.Platform.Networking.Exceptions;
using Blockcore.Platform.Networking.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Blockcore.Platform.Networking
{
    public class MessageProcessing : IMessageProcessingBase
    {
        private readonly IEnumerable<IMessageHandler> messageHandlers;
        private readonly MessageMaps messageMaps;

        public MessageProcessing(IEnumerable<IMessageHandler> messageHandlers, MessageMaps messageMaps)
        {
            this.messageHandlers = messageHandlers;
            this.messageMaps = messageMaps;
        }

        /// <summary>
        /// Important to call before any processing, to ensure all messages and handlers are registered.
        /// </summary>
        public void Build()
        {
            foreach (var handler in messageHandlers)
            {
                var handlerType = handler.GetType();
                var type = handlerType.GetInterface("IHandle`1").GetGenericArguments().First();

                // While we previously had MessageAttribute to get the Command, changes to MessagePack and use of property, we must 
                // create an instance of the message. Since this method is only called at startup and not while processing messages,
                // it shouldn't matter much on performance.
                BaseMessage instance = (BaseMessage)Activator.CreateInstance(type);
                var prop = type.GetProperty("Command");
                var cmd = (ushort)prop.GetValue(instance);

                if (!messageMaps.Contains(cmd))
                {
                    var map = new Map();
                    map.Command = cmd;
                    map.MessageType = type;
                    map.Handlers.Add(handler);
                    messageMaps.AddCommand(cmd, map);
                }
                else
                {
                    messageMaps.AddHandler(cmd, handler);
                }
            }
        }

        /// <summary>
        /// Processes incoming messages and delegates the processing to all registered message handlers. It is possible for multiple handlers to process the same message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="protocol"></param>
        /// <param name="endpoint"></param>
        /// <param name="client"></param>
        public void Process(BaseMessage message, ProtocolType protocol, IPEndPoint endpoint = null, TcpClient client = null)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (!messageMaps.Contains(message.Command))
            {
                throw new MessageProcessingException($"No handlers for message {message.Command}");
            }

            var map = messageMaps.GetMap(message.Command);

            foreach (dynamic handler in map.Handlers)
            {
                handler.Process(message, protocol, endpoint, client);
            }
        }
    }
}
