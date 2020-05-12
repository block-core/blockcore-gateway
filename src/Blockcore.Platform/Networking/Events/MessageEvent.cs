using Blockcore.Platform.Networking.Entities;

namespace Blockcore.Platform.Networking.Events
{
    public class MessageReceivedEvent
    {
        public string From { get; set; }

        public string To { get; set; }

        public string Content { get; set; }

        public Message Data { get; set; }
    }
}
