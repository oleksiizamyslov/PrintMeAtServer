namespace PrintMeAtServer.Core.Interfaces
{
    /// <summary>
    /// All redis specific settings go here.
    /// </summary>
    public interface IRedisConfiguration
    {
        string RedisServerHostname { get; }
        int RedisServerPort { get; }
    }
}