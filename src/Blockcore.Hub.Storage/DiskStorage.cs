using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Blockcore.Configuration;
using Blockcore.Settings;
using Newtonsoft.Json;

namespace Blockcore.Hub.Storage
{
   public class DiskStorageDocument
   {
      // [JsonPropertyName("id")]
      public string Id { get; set; }

      // [JsonPropertyName("type")]
      public string Type { get; set; }

      // [JsonPropertyName("signature")]
      public string Signature { get; set; }

      // [JsonPropertyName("contentType")]
      public string ContentType { get; set; }
   }

   public class StorageDocumentInfo
   {
      public DateTime LastWriteTime { get; set; }

      public DateTime CreationTime { get; set; }

      public string Name { get; set; }

      public long Size { get; set; }
   }

   /// <summary>
   /// Basic storage implementation for Hubs that writes files to local disk.
   /// </summary>
   public class DiskStorage : IHubStorage
   {
      private readonly HubSettings settings;
      private readonly DirectoryInfo dataFolder;

      public DiskStorage(HubSettings settings)
      {
         this.settings = settings;
         dataFolder = new DirectoryInfo(settings.DataFolder);
      }

      public async Task StartAsync()
      {
         if (!dataFolder.Exists)
         {
            dataFolder.Create();
         }
      }

      public void Save(Document document)
      {
         DirectoryInfo dir = new DirectoryInfo(Path.Combine(dataFolder.FullName, document.Type));

         if (!dir.Exists)
         {
            dir.Create();
         }

         var doc = new DiskStorageDocument
         {
            Id = document.Id,
            Type = document.Type,
            ContentType = document.ContentType,
            Signature = document.Signature
         };

         string json = JsonConvert.SerializeObject(doc);

         FileInfo file = new FileInfo(Path.Combine(dir.FullName, document.Id));

         string metaFile = file.FullName + ".json";
         string dataFile = file.FullName + ".bin";

         File.WriteAllText(metaFile, json);
         File.WriteAllBytes(dataFile, document.Data);
      }

      public Document Load(string type, string id)
      {
         DirectoryInfo dir = new DirectoryInfo(Path.Combine(dataFolder.FullName, type));

         if (!dir.Exists)
         {
            return null;
         }

         FileInfo file = new FileInfo(Path.Combine(dir.FullName, id));

         string metaFile = file.FullName + ".json";
         string dataFile = file.FullName + ".bin";

         if (!File.Exists(dataFile))
         {
            return null;
         }

         Document document = JsonConvert.DeserializeObject<Document>(File.ReadAllText(metaFile));
         byte[] data = File.ReadAllBytes(dataFile);
         document.Data = data;
         return document;
      }

      public IEnumerable<StorageDocumentInfo> List(string type)
      {
         DirectoryInfo dir = new DirectoryInfo(Path.Combine(dataFolder.FullName, type));

         if (!dir.Exists)
         {
            return null;
         }

         FileInfo[] files = dir.GetFiles();

         IEnumerable<StorageDocumentInfo> fileNames = files.Select(f => new StorageDocumentInfo
         {
            Name = f.Name,
            CreationTime = f.CreationTimeUtc,
            LastWriteTime = f.LastWriteTimeUtc,
            Size = f.Length
         });

         return fileNames;
      }
   }
}
