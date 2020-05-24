using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;

namespace Blockcore.Platform.Tests.Fakes
{
   public class TestLogger : ILogger
   {
      private readonly ITestOutputHelper output;
      private readonly string category;
      private readonly string prefix;

      public TestLogger(string category, string prefix, ITestOutputHelper output)
      {
         this.category = category;
         this.prefix = prefix;

         this.output = output;
      }

      public IDisposable BeginScope<TState>(TState state)
      {
         return new NoopDisposable();
      }

      public bool IsEnabled(LogLevel logLevel)
      {
         return true;
      }

      public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
      {
         string message = string.Empty;

         if (formatter != null)
         {
            message += formatter(state, exception);
         }

         output.WriteLine($"{logLevel.ToString()} - {eventId.Id} - {category} - {message}");
      }

      private class NoopDisposable : IDisposable
      {
         public void Dispose()
         {
         }
      }
   }
}
