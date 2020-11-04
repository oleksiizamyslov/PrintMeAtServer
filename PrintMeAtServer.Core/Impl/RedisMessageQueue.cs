using System;
using System.Linq;
using System.Threading.Tasks;
using PrintMeAtServer.Core.Interfaces;
using PrintMeAtServer.Core.Models;
using StackExchange.Redis;

namespace PrintMeAtServer.Core.Impl
{
    public class RedisMessageQueue : IMessageQueue
    {
        private readonly IRedisConnectionFactory _factory;
        private readonly ISerializer<MessageWrapper> _serializer;
        private readonly IServerConfiguration _configuration;

        public RedisMessageQueue(IRedisConnectionFactory factory, ISerializer<MessageWrapper> serializer, IServerConfiguration configuration)
        {
            _factory = factory;
            _serializer = serializer;
            _configuration = configuration;
        }

        /// <summary>
        /// Simple sharding by server name is used here. This only works for dedicated, named servers.
        /// If we will be hosting in the cloud, we will have to come up with a more stable approach than this.
        /// </summary>
        private string RedisKey => $"PrintMessages_{_configuration.ServerName}";

        public async Task<Message> PeekNextScheduledMessage()
        {
            return await GetLatestMessage(false);
        }

        public async Task EnqueueMessage(Message message)
        {
            var serialized = await _serializer.Serialize(new MessageWrapper(message));

            var rv = new RedisValue(serialized);
            var database = _factory.GetDatabase();

            var score = message.DateTime.DateTime.ToOADate();
            database.SortedSetAdd(RedisKey, rv, score);
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

            return message.Message;
        }

        /// <summary>
        /// Wrap message to make sure messages with same date and text are treated as different.
        /// </summary>
        public class MessageWrapper
        {
            public MessageWrapper()
            {

            }

            public MessageWrapper(Message message)
            {
                Message = message;
                Id = Guid.NewGuid();
            }

            public Message Message { get; set; }
            public Guid Id { get; set; }
        }
    }
}