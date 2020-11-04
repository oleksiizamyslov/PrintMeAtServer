using System;

namespace PrintMeAtServer.Core.Interfaces
{
    public interface IDateTimeProvider
    {
        DateTimeOffset Now { get; }
    }
}