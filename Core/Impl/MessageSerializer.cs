using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Data;
using Core.Interfaces;

namespace Core.Impl
{
    public class MessageSerializer : IMessageSerializer
    {
        public async Task<string> Serialize(Message message)
        {
            if (message == null)
                return null;
            using (var stream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(stream, message, message.GetType());
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        public async Task<Message> Deserialize(string serialized)
        {
            if (serialized == null)
                return null;
            var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(serialized));
            return await JsonSerializer.DeserializeAsync<Message>(ms);
        }
    }
}