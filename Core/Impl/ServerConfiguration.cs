using System;
using Core.Interfaces;

namespace Core.Impl
{
    public class ServerConfiguration : IServerConfiguration
    {
        public string ServerName => Environment.MachineName;
    }
}
