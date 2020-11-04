using System;

namespace Core.Interfaces
{
    public interface ITimerFactory
    {
        IOneOffTimer Create(Action timerCallback);
    }
}