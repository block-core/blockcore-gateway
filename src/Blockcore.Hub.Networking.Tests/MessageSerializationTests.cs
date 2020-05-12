using Blockcore.Platform.Networking;
using Blockcore.Platform.Networking.Handlers;
using Blockcore.Platform.Networking.Messages;
using System;
using System.IO;
using Xunit;

namespace Blockcore.Platform.Tests
{
    public class MessageSerializationTests
    {
        [Fact]
        public void ShouldSerializeMessages()
        {
            var msg = new TestMessage()
            {
                CustomInt = 123,
                CustomString = "Hello",
                Endpoint = "0.0.0.0:6000"
            };

            var maps = new MessageMaps();
            maps.AddCommand(MessageTypes.TEST, new Map() { Command = MessageTypes.TEST, MessageType = typeof(TestMessage) });
            maps.AddHandler(MessageTypes.TEST, new TestMessageHandler());

            var serializer = new MessageSerializer(maps);

            var serialized = serializer.Serialize(msg);
            var deserialized = serializer.Deserialize<TestMessage>(serialized);

            Assert.True(msg.Equals(deserialized));

            var stream = new MemoryStream();
            serializer.Serialize(msg, stream);
            stream.Seek(0, SeekOrigin.Begin);
            deserialized = serializer.Deserialize(stream);

            Assert.True(msg.Equals(deserialized));
        }
    }
}
