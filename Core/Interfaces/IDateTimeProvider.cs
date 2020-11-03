using System;

namespace Core
{
    public interface IDateTimeProvider
    {
        DateTimeOffset Now { get; }
    }
}