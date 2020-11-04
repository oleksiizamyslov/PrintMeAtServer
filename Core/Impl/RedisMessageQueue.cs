using System.Linq;
using System.Threading.Tasks;
using Core.Data;
using Core.Interfaces;
using StackExchange.Redis;

namespace Core.Impl
{
    public class RedisMessageQueue : IMessageQueue
    {
        private readonly IRedisConnectionFactory _factory;
        private readonly IMessageSerializer _serializer;
        private readonly IServerConfiguration _configuration;

        public RedisMessageQueue(IRedisConnectionFactory factory, IMessageSerializer serializer, IServerConfiguration configuration)
        {
            _factory = factory;
            _serializer = serializer;
            _configuration = configuration;
        }

        public async Task<Message> PeekNextScheduledMessage()
        {
            return await GetLatestMessage(false);
        }

        public async Task EnqueueMessage(Message message)
        {
            var serialized = await _serializer.Serialize(message);
            var rv = new RedisValue(serialized);
            var database = _factory.GetDatabase();
            database.SortedSetAdd(RedisKey, rv, message.DateTime.DateTime.ToOADate());
        }

        public async Task<Message> DequeueNextScheduledMessage()
        {
            return await GetLatestMessage(true);
        }

        private async Task<Message> GetLatestMessage(bool pop)
        {
            var database = _factory.GetDatabase();
            var keys = database.SortedSetRangeByRank(RedisKey, 0, 0);
            if (keys.Length == 0)
            {
                return null;
            }

            var value = keys.Single();
            
            var serialized = value.ToString();
            var message = await _serializer.Deserialize(serialized);
            if (pop)
            {
                database.SortedSetRemove(RedisKey, value);
            }

            return message;
        }
        
        private string RedisKey => $"PrintMessages_{_configuration.ServerName}";
    }
}