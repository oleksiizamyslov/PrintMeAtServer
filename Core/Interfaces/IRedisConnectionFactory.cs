using StackExchange.Redis;

namespace Core.Interfaces
{
    public interface IRedisConnectionFactory
    {
        IDatabase GetDatabase();
    }
}