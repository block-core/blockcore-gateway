using Blockcore.Platform.Networking.Messages;
using MessagePack;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Blockcore.Platform.Networking
{
    public class MessageSerializer
    {
        private int HeaderSize;
        private const ushort Version = 1;
        private int VersionByteSize;
        private readonly MessageMaps maps;

        public MessageSerializer(MessageMaps maps)
        {
            this.maps = maps;
            HeaderSize = Unsafe.SizeOf<HeaderInfo>();
            VersionByteSize = Unsafe.SizeOf<ushort>();
        }

        public byte[] Serialize(BaseMessage message)
        {
            var messageDataBytes = MessagePackSerializer.Serialize(message.GetType(), message);

            var header = new HeaderInfo()
            {
                Command = message.Command,
                Size = messageDataBytes.Length
            };

            var span = MemoryMarshal.CreateSpan(ref header, 1);
            var headerBytes = MemoryMarshal.AsBytes(span);
            var newData = new byte[VersionByteSize + headerBytes.Length + messageDataBytes.Length];

            // Treat version as a separate entity because the header struct might not be the same between versions
            Buffer.BlockCopy(BitConverter.GetBytes(Version), 0, newData, 0, VersionByteSize);
            Buffer.BlockCopy(headerBytes.ToArray(), 0, newData, VersionByteSize, headerBytes.Length);

            // msgpack is very robust though so it can handle different versions of the same object as long as you keep adding keys and not removing them
            Buffer.BlockCopy(messageDataBytes, 0, newData, VersionByteSize + headerBytes.Length, messageDataBytes.Length);

            return newData;
        }

        public void Serialize(BaseMessage message, Stream stream)
        {
            var messageDataBytes = MessagePackSerializer.Serialize(message.GetType(), message);

            var header = new HeaderInfo()
            {
                Command = message.Command,
                Size = messageDataBytes.Length
            };

            var span = MemoryMarshal.CreateSpan(ref header, 1);
            var headerBytes = MemoryMarshal.AsBytes(span);

            // Treat version as a separate entity because the header struct might not be the same between versions
            stream.Write(new Span<byte>(BitConverter.GetBytes(Version)));

            stream.Write(headerBytes);

            // msgpack is very robust though so it can handle different versions of the same object as long as you keep adding keys and not removing them
            stream.Write(messageDataBytes);
        }

        public BaseMessage Deserialize(byte[] data)
        {
            var stream = new MemoryStream(data);
            return Deserialize(stream);
        }

        public BaseMessage Deserialize(Stream stream)
        {
            var reader = new BinaryReader(stream);

            // Make sure we handle versioning from 
            var version = reader.ReadUInt16();

            // TODO: Move versioning to MESSAGES, we don't want hubs to be incompatible on upgrades, but ensure that older hubs to still be able to processes older messages.
            if (!version.Equals(Version)) // oh no! But you can handle different protocol versions. This is very strict
                throw new Exception($"Invalid version. Expected {Version} but got {version}");

            var header = new HeaderInfo();
            var headerSpan = MemoryMarshal.CreateSpan(ref header, 1);
            var headerByteSpan = MemoryMarshal.AsBytes(headerSpan);
            reader.Read(headerByteSpan);

            // Get the registered message type, these are registered using dependency injection.
            var messageType = maps.GetMessageType(header.Command);

            // Read the full message, based on the size provided in the header.
            var messageBytes = reader.ReadBytes(header.Size);

            var message = (BaseMessage)MessagePackSerializer.Deserialize(messageType, messageBytes);

            return message;
        }

        public BaseMessage Deserialize<T>(byte[] data) where T: BaseMessage
        {
            var version = BitConverter.ToUInt16(data, 0);

            if (!version.Equals(Version)) // oh no! But you can handle different protocol versions. This is very strict
                throw new Exception($"Invalid version. Expected {Version} but got {version}");

            var header = new HeaderInfo();
            var headerSpan = MemoryMarshal.CreateSpan(ref header, 1);
            var headerByteSpan = MemoryMarshal.AsBytes(headerSpan);

            var inputSpan = new Span<byte>(data);
            inputSpan.Slice(VersionByteSize, HeaderSize).CopyTo(headerByteSpan); // Read header

            // Get the registered message type, these are registered using dependency injection.
            var messageType = maps.GetMessageType(header.Command);

            // Don't slice and convert to byte array before it's necessary
            var messageBytes = inputSpan.Slice(VersionByteSize + HeaderSize, header.Size).ToArray();

            var message = (BaseMessage)MessagePackSerializer.Deserialize(messageType, messageBytes);

            return message;
        }
    }
}
