using StackExchange.Redis;

namespace Core
{
    public interface IRedisConnectionFactory
    {
        IDatabase GetDatabase();
    }
}