using System;
using Core.Interfaces;

namespace Test.Infrastructure
{
    public class SyncTimerFactory : ITimerFactory
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public SyncTimer(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }
        public IOneOffTimer Create(Action timerCallback)
        {
            return new SyncTimer(timerCallback, _dateTimeProvider);
        }
    }
}