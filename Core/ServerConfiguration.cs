using System;

namespace Core
{
    public class ServerConfiguration : IServerConfiguration
    {
        public string ServerName => Environment.MachineName;
    }
}
