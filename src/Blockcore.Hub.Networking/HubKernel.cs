using System;
using System.Collections.Generic;
using System.Text;
using Blockcore.Hub.Networking.Entities;
using Blockcore.Platform.Networking.Entities;
using Microsoft.AspNetCore.DataProtection;

namespace Blockcore.Hub.Networking
{
   /// <summary>
   /// Main orchestrator of logic on a running instance of the Blockcore Hub.
   /// </summary>
   public class HubKernel
   {
      private readonly Identity identity;
      private Profile profile;

      public HubKernel(Identity identity)
      {
         this.identity = identity;
         Id = this.identity.Id;

         Invitations = new List<HubInvitation>();
         Hubs = new List<HubInstance>();
      }

      public string Id { get; private set; }

      /// <summary>
      /// Current list of incoming invitations to exchange keys.
      /// </summary>
      public List<HubInvitation> Invitations { get; set; }

      /// <summary>
      /// List of hubs that invitation has been accepted for.
      /// </summary>
      public List<HubInstance> Hubs { get; set; }

      public Profile Profile { get; set; }

      /// <summary>
      /// Initializes the Hub Kernel by loading all state from storage.
      /// </summary>
      public void Initialize()
      {

      }

      /// <summary>
      /// Will load profile from storage first and then initialize query against the network to get updated profile data.
      /// </summary>
      public void LoadProfile(Profile profile = null)
      {
         this.profile = profile;
      }

      public HubInvitation CreateInvite(string id, string v)
      {
         var invite = new HubInvitation { Created = DateTime.UtcNow, Direction = InvitationDirection.Outgoing, HubId = this.Id };




         return invite;
      }

      /// <summary>
      /// Accepts an invite and adds to the list of trusted hubs. This method does not verify the signature or existance in the current list of invitations, will remove if exists.
      /// </summary>
      /// <param name="invite"></param>
      public void AcceptInvite(HubInvitation invite)
      {
         var hubInstance = new HubInstance(invite.HubId);
         hubInstance.Created = DateTime.UtcNow;

         Hubs.Add(hubInstance);

         Invitations.Remove(invite);

         Persist();
      }

      public void Persist()
      {
         // Persist state.
      }
   }
}
