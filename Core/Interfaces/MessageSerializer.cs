using System.Text.Json;

namespace Core
{
    public class MessageSerializer : IMessageSerializer
    {
        public string Serialize(Message message)
        {
            if (message == null)
                return null;
            return JsonSerializer.Serialize(message);
        }

        public Message Deserialize(string serialized)
        {
            if (serialized == null)
                return null;
            return JsonSerializer.Deserialize<Message>(serialized);
        }
    }
}