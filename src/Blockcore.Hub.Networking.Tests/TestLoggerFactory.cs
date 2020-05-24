using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;

namespace Blockcore.Platform.Tests.Fakes
{
   public class TestLoggerFactory : ILoggerFactory
   {
      private readonly ITestOutputHelper output;

      public TestLoggerFactory(ITestOutputHelper output)
      {
         this.output = output;
      }

      public void AddProvider(ILoggerProvider provider)
      {

      }

      public ILogger CreateLogger(string categoryName)
      {
         return new TestLogger(categoryName, "", output);
      }

      public void Dispose()
      {

      }
   }
}
