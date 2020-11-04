using System;

namespace Core.Interfaces
{
    public interface IDateTimeProvider
    {
        DateTimeOffset Now { get; }
    }
}