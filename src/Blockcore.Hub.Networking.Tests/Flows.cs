using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Blockcore.Hub.Networking.Entities;
using Blockcore.Hub.Networking.Managers;
using NBitcoin;
using Xunit;

namespace Blockcore.Hub.Networking.Tests
{
   /// <summary>
   /// Integration tests that implements the full flow of networking and application logic for Blockcore Hub.
   /// </summary>
   public class Flows
   {
      public Identity RandomIdentity()
      {
         var recoveryPhrase = new Mnemonic(Wordlist.English, WordCount.Twelve);
         Identity identity = new Identity(recoveryPhrase.ToString());
         return identity;
      }

      [Fact]
      public void FullIntegration()
      {
         Identity identity = RandomIdentity();
         Identity identity01 = RandomIdentity();
         Identity identity02 = RandomIdentity();
         Identity identity03 = RandomIdentity();

         HubKernel hub = new HubKernel(identity);
         HubKernel hub01 = new HubKernel(identity01);
         HubKernel hub02 = new HubKernel(identity02);
         HubKernel hub03 = new HubKernel(identity03);

         // Load the profile from disk, and then from the network. This is the profile of this running instance of the Hub.
         Profile profile = new Profile
         {
            Id = identity.Id,
            Name = "John Doe",
            Shortname = "JD",
            Alias = "johndoe",
            Title = "I'm lost in translation.",
            Timestamp = DateTime.UtcNow
         };

         hub.LoadProfile(profile);
         Assert.Equal("JD", hub.Profile.Shortname);

         // Create an invite between hub01 and hub.
         HubInvitation invite = hub01.CreateInvite(hub.Id, "Hi, hope we can connect?");
          hub01.Sign(invite);


         HubKernel kernel = new HubKernel(identity);

         kernel.Invitations.Add(new HubInvitation { Direction = InvitationDirection.Incoming, Created = DateTime.UtcNow, HubId = hub01.Identity.Id });
         kernel.Invitations.Add(new HubInvitation { Direction = InvitationDirection.Incoming, Created = DateTime.UtcNow, HubId = hub02.Id });
         kernel.Invitations.Add(new HubInvitation { Direction = InvitationDirection.Incoming, Created = DateTime.UtcNow, HubId = hub03.Id });

         kernel.AcceptInvite(kernel.Invitations[0]);

         Assert.Equal(2, kernel.Invitations.Count);
         Assert.Single(kernel.Hubs);

         

         
         kernel.Initialize();

      }
   }
}
