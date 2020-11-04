using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using PrintMeAtServer.Core.Interfaces;

namespace PrintMeAtServer.Core.Impl
{
    public class Serializer<T> : ISerializer<T>
    {
        public async Task<string> Serialize(T obj)
        {
            if (obj == null)
                return null;
            using (var stream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(stream, obj, obj.GetType());
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        public async Task<T> Deserialize(string serialized)
        {
            if (serialized == null)
                return default;
            var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(serialized));
            return await JsonSerializer.DeserializeAsync<T>(ms);
        }
    }
}