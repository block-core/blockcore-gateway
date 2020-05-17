namespace Blockcore.Platform.Networking.Events
{
   public class GatewayErrorEvent
   {
      public string Message { get; set; }

      public GatewayErrorEvent(string message)
      {
         Message = message;
      }
   }
}
