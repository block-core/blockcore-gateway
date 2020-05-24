using System;
using System.Collections.Generic;
using System.Text;
using Karambolo.Extensions.Logging.File;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockcore.Hub.Networking.Providers
{
   [ProviderAlias("ErrorFile")]
   public class FileLoggerProvider : Karambolo.Extensions.Logging.File.FileLoggerProvider
   {
      public FileLoggerProvider(FileLoggerContext context, IOptionsMonitor<FileLoggerOptions> options, string optionsName) : base(context, options, optionsName) { }
   }
}
