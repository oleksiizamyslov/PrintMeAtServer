using System;
using System.Net;
using PrintMeAtServer.Core.Interfaces;
using StackExchange.Redis;

namespace PrintMeAtServer.Core.Impl
{
    public class RedisConnectionFactory : IRedisConnectionFactory, IDisposable
    {
        private readonly IRedisConfiguration _configuration;
        private readonly object _lock = new object();

        public RedisConnectionFactory(IRedisConfiguration configuration)
        {
            _configuration = configuration;
        }

        private ConnectionMultiplexer _connectionMultiplexer;

        private ConnectionMultiplexer GetMultiplexer()
        {
            if (_connectionMultiplexer == null)
            {
                lock (_lock)
                {
                    if (_connectionMultiplexer == null)
                    {
                        var endpoint = new DnsEndPoint(_configuration.RedisServerHostname,
                            _configuration.RedisServerPort);
                        try
                        {
                            _connectionMultiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions()
                            {
                                EndPoints = {endpoint}
                            });
                        }
                        catch (RedisConnectionException ex)
                        {
                            throw new InvalidOperationException($"Unable to connect to redis server at '{_configuration.RedisServerHostname}:{_configuration.RedisServerPort}'.", ex);
                        }
                    }
                }
            }

            if (!_connectionMultiplexer.IsConnected)
            {
                throw new InvalidOperationException($"Connection lost to Redis instance at '{_configuration.RedisServerHostname}:{_configuration.RedisServerPort}'");
            }

            return _connectionMultiplexer;
        }

        public IDatabase GetDatabase()
        {
            return GetMultiplexer().GetDatabase(0);
        }
        public void Dispose()
        {
            _connectionMultiplexer?.Dispose();
            _connectionMultiplexer = null;
        }
    }
}