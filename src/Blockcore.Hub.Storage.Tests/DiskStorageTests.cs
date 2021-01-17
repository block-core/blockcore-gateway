
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Blockcore.Hub.Networking;
using Blockcore.Hub.Networking.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using NBitcoin;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Xunit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Blockcore.Hub.Networking.Entities;

namespace Blockcore.Hub.Storage.Tests
{


   public class StorageTests
   {
      [Fact]
      public void CreateAndVerifyStorage()
      {
         IHost host = Blockcore.Hub.Program.CreateHostBuilder(new string[] { }).Build();
         IHubStorage storage = host.Services.GetService<IHubStorage>();
         IOptions<Settings.HubSettings> options = host.Services.GetService<IOptions<Settings.HubSettings>>();
         Settings.HubSettings settings = options.Value;

         JsonConvert.DefaultSettings = () =>
         {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter());
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return settings;
         };

         storage.StartAsync().Wait();

         Protection protection = new Protection();
         Mnemonic recoveryPhrase;

         string path = Path.Combine(settings.DataFolder, "recoveryphrase.txt");

         if (!File.Exists(path))
         {
            recoveryPhrase = new Mnemonic(Wordlist.English, WordCount.Twelve);
            string cipher = protection.Protect(recoveryPhrase.ToString());
            File.WriteAllText(path, cipher);
         }
         else
         {
            string cipher = File.ReadAllText(path);
            recoveryPhrase = new Mnemonic(protection.Unprotect(cipher));
         }

         Identity identity = new Identity(recoveryPhrase.ToString());

         Profile profile = new Profile
         {
            Id = identity.Id,
            Name = "Sondre Bjellås",
            Shortname = "Sondre",
            Alias = "sondreb",
            Title = "I do a little bit of everything.",
            Timestamp = DateTime.UtcNow
         };

         // Get the payload to be signed or encrypted.
         byte[] data = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(profile));

         // Sign the payload.
         string signature = identity.Sign(data);

         Document document = new Document
         {
            Id = identity.Id,
            ContentType = "application/json",
            Signature = signature,
            Type = "identity",
            Data = data
         };

         storage.Save(document);

         identity = new Identity(recoveryPhrase.ToString(), 1);

         profile = new Profile
         {
            Id = identity.Id,
            Name = "John Doe",
            Shortname = "JD",
            Alias = "JD",
            Title = "Unknown",
            Timestamp = DateTime.UtcNow
         };

         // Get the payload to be signed or encrypted.
         data = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(profile));

         // Sign the payload.
         signature = identity.Sign(data);

         document = new Document
         {
            Id = identity.Id,
            ContentType = "application/json",
            Signature = signature,
            Type = "identity",
            Data = data
         };

         storage.Save(document);

         Document persistedProfile = storage.Load("identity", identity.Id);

         Assert.Equal(persistedProfile.Signature, signature);
         Assert.Equal(persistedProfile.Data, data);

         StorageDocumentInfo[] identities = storage.List("identity").ToArray();

         Assert.Equal(4, identities.Length);
      }
   }
}
