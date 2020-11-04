using System;

namespace PrintMeAtServer.Core.Interfaces
{
    public interface ITimerFactory
    {
        IOneOffTimer Create(Action timerCallback);
    }
}