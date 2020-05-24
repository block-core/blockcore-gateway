using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;

namespace Blockcore.Platform.Tests
{
   public class TestBase
   {
      private readonly ITestOutputHelper output;

      public TestBase(ITestOutputHelper output)
      {
         this.output = output;
      }
   }
}
