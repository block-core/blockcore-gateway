using Blockcore.Platform.Networking.Exceptions;
using Blockcore.Platform.Networking.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Blockcore.Platform.Networking
{
   public class GatewayMessageProcessing : IHubMessageProcessing, IGatewayMessageProcessing
   {
      private readonly IEnumerable<IMessageHandler> messageHandlers;
      private readonly MessageMaps maps;

      public GatewayMessageProcessing(IEnumerable<IGatewayMessageHandler> messageHandlers)
      {
         this.messageHandlers = messageHandlers;
         maps = new MessageMaps();
      }

      /// <summary>
      /// Important to call before any processing, to ensure all messages and handlers are registered.
      /// </summary>
      public MessageMaps Build()
      {
         foreach (IMessageHandler handler in messageHandlers)
         {
            Type handlerType = handler.GetType();
            Type type = handlerType.GetInterface("IHandle`1").GetGenericArguments().First();

            // While we previously had MessageAttribute to get the Command, changes to MessagePack and use of property, we must 
            // create an instance of the message. Since this method is only called at startup and not while processing messages,
            // it shouldn't matter much on performance.
            BaseMessage instance = (BaseMessage)Activator.CreateInstance(type);
            System.Reflection.PropertyInfo prop = type.GetProperty("Command");
            ushort cmd = (ushort)prop.GetValue(instance);

            if (!maps.Contains(cmd))
            {
               var map = new Map();
               map.Command = cmd;
               map.MessageType = type;
               map.Handlers.Add(handler);
               maps.AddCommand(cmd, map);
            }
            else
            {
               maps.AddHandler(cmd, handler);
            }
         }

         return maps;
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

         if (!maps.Contains(message.Command))
         {
            throw new MessageProcessingException($"No handlers for message {message.Command}");
         }

         Map map = maps.GetMap(message.Command);

         foreach (dynamic handler in map.Handlers)
         {
            handler.Process(message, protocol, endpoint, client);
         }
      }
   }
}
