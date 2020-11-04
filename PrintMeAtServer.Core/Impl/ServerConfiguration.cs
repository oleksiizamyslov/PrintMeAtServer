using System;
using PrintMeAtServer.Core.Interfaces;

namespace PrintMeAtServer.Core.Impl
{
    public class ServerConfiguration : IServerConfiguration
    {
        public string ServerName => Environment.MachineName;
    }
}
