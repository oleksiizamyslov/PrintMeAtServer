using System;

namespace Core.Interfaces
{
    public interface IOneOffTimer : IDisposable
    {
        DateTimeOffset CurrentlyScheduledTime { get; }
        void Reschedule(DateTimeOffset newMessageOffset);
    }
}