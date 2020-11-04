using System;
using Core.Interfaces;

namespace Core.Impl
{
    public class TimerFactory:ITimerFactory
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public TimerFactory(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public IOneOffTimer Create(Action timerCallback)
        {
            return new OneOffTimer(timerCallback, _dateTimeProvider);
        }
    }
}
