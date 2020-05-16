using System;
using System.Collections.Generic;
using System.Reflection;
using Blockcore.Hub.Networking.Managers;
using Blockcore.Utilities;
using Newtonsoft.Json;

namespace Blockcore.Hub.Networking.Hubs
{
   public class CommandResult
   {
      public string Service { get; set; }

      public string Command { get; set; }

      public object Result { get; set; }
   }

   public class CommandDispatcher
   {
      private readonly IServiceProvider serviceProvider;

      private readonly Dictionary<string, object> services;

      public CommandDispatcher(IServiceProvider serviceProvider, HubManager hubManager)
      {
         // White-listing of services made available through the dispatcher.
         services = new Dictionary<string, object>();
         services.Add("HubManager", hubManager);
         //services.Add("NodeLifetime", nodeLifetime);

         this.serviceProvider = serviceProvider;
      }

      public string Execute(string service, string command, object[]? args)
      {
         object instance = services[service];
         Type type = instance.GetType();
         MethodInfo methodInfo = type.GetMethod(command);
         object result = methodInfo.Invoke(instance, args);

         var envelope = new CommandResult();
         envelope.Service = service;
         envelope.Command = command;
         envelope.Result = result;

         return JsonConvert.SerializeObject(envelope);
      }
   }
}
