using System;

namespace PrintMeAtServer.Core.Interfaces
{
    public interface IOneOffTimer : IDisposable
    {
        DateTimeOffset CurrentlyScheduledTime { get; }
        void Reschedule(DateTimeOffset newMessageOffset);
    }
}