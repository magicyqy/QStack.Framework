using DotNetCore.CAP.Messages;
using MessagePack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.Docker.Crawler
{
    public class CapMessagePackSerializer : DotNetCore.CAP.Serialization.ISerializer
    {
        private static readonly MessagePackSerializerOptions SerializerOptions =
            MessagePackSerializer.Typeless.DefaultOptions.WithCompression(MessagePackCompression.Lz4Block);
        public Message Deserialize(string json)
        {
            throw new NotImplementedException();
        }

        public object Deserialize(object value, Type valueType)
        {
            throw new NotImplementedException();
        }

        public async Task<Message> DeserializeAsync(TransportMessage transportMessage, Type valueType)
        {
            if (valueType == null || transportMessage.Body == null)
            {
                return await Task.FromResult(new Message(transportMessage.Headers, null));
            }

            var stream = new MemoryStream(transportMessage.Body);
            var obj = await MessagePackSerializer.Typeless.DeserializeAsync(stream, SerializerOptions);
            return await Task.FromResult(new Message(transportMessage.Headers, obj));
        }

        public bool IsJsonType(object jsonObject)
        {
            return false;
        }

        public string Serialize(Message message)
        {
            message.Value = MessagePackSerializer.Typeless.Serialize(message.Value, SerializerOptions);
            return JsonConvert.SerializeObject(message);
        }

        public async Task<TransportMessage> SerializeAsync(Message message)
        {
            Console.WriteLine("start pack message..........");
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (message.Value == null)
            {
                return await Task.FromResult(new TransportMessage(message.Headers, null));
            }

            var bytes = MessagePackSerializer.Typeless.Serialize(message.Value, SerializerOptions);
            return await Task.FromResult(new TransportMessage(message.Headers, bytes));
        }
    }
}
