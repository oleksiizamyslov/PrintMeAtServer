using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Core
{
    public class PersistentRedisMessageQueue : IPersistentMessageQueue
    {
        private readonly IRedisConnectionFactory _factory;
        private readonly IMessageSerializer _serializer;
        private readonly IServerConfiguration _configuration;

        public PersistentRedisMessageQueue(IRedisConnectionFactory factory, IMessageSerializer serializer, IServerConfiguration configuration)
        {
            _factory = factory;
            _serializer = serializer;
            _configuration = configuration;
        }

        public Task Initialize()
        {
            return Task.CompletedTask;
        }
        
        public Task<Message> PeekNextScheduledMessage()
        {
            return Task.FromResult(GetLatestMessage(false));
        }

        public Task EnqueueMessage(Message message)
        {
            var serialized = _serializer.Serialize(message);
            var rv = new RedisValue(serialized);
            var database = _factory.GetDatabase();
            database.SortedSetAdd(RedisKey, rv, message.DateTime.DateTime.ToOADate());
            return Task.CompletedTask;
        }

        public Task<Message> PopNextScheduledMessage()
        {
            return Task.FromResult(GetLatestMessage(true));
        }

        private Message GetLatestMessage(bool pop)
        {
            var database = _factory.GetDatabase();
            var keys = database.SortedSetRangeByRank(RedisKey, 0, 0);
            if (keys.Length == 0)
            {
                return null;
            }

            var value = keys.Single();
            
            var serialized = value.ToString();
            var message = _serializer.Deserialize(serialized);
            if (pop)
            {
                database.SortedSetRemove(RedisKey, value);
            }

            return message;
        }
        
        private string RedisKey => $"PrintMessages_{_configuration.ServerName}";
    }
}