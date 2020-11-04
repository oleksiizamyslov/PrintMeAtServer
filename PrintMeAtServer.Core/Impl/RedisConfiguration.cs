using System;
using System.Data;
using Microsoft.Extensions.Configuration;
using PrintMeAtServer.Core.Interfaces;

namespace PrintMeAtServer.Core.Impl
{
    public class RedisConfiguration : IRedisConfiguration
    {
        private readonly IConfiguration _configuration;

        public RedisConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public int RedisServerPort
        {
            get
            {
                var port = GetRequiredSetting("RedisPort");

                try
                {
                    return Convert.ToInt32(port);
                }
                catch
                {
                    throw new InvalidOperationException($"Configured redis port value '{port}' is invalid. Integer value expected.");
                }
            }
        }

        public string RedisServerHostname => _configuration["RedisHost"];

        private string GetRequiredSetting(string name)
        {
            var setting = _configuration[name];
            if (setting == null)
            {
                throw new InvalidConstraintException($"Configuration setting '{name}' is missing in the project configuration.");
            }

            return setting;
        }
    }
}