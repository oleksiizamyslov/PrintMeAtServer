using StackExchange.Redis;

namespace PrintMeAtServer.Core.Interfaces
{
    public interface IRedisConnectionFactory
    {
        IDatabase GetDatabase();
    }
}