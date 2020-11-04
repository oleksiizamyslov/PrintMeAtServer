using System;
using System.Net;
using Core.Interfaces;
using StackExchange.Redis;

namespace Core.Impl
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
                        _connectionMultiplexer = ConnectionMultiplexer.Connect("redis-container:6379");
                    }
                }
            }

            if (!_connectionMultiplexer.IsConnected)
            {
                throw new InvalidOperationException($"Unable to connect to Redis instance at '{_connectionMultiplexer.GetEndPoints()[0]}'");
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