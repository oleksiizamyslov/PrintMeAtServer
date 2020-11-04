using System;
using System.Threading;
using PrintMeAtServer.Core.Interfaces;

namespace PrintMeAtServer.Core.Impl
{
    public class OneOffTimer: IOneOffTimer, IDisposable
    {
        private readonly Action _action;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly Timer _timer;
        
        public OneOffTimer(Action action, IDateTimeProvider dateTimeProvider)
        {
            _action = action;
            _dateTimeProvider = dateTimeProvider;
            _timer = new Timer((st) => DoWork(), null, Timeout.Infinite, Timeout.Infinite);
            SetIdle();
        }

        public DateTimeOffset CurrentlyScheduledTime { get; private set; }

        public virtual void Reschedule(DateTimeOffset offset)
        {
            if (offset > MaxPossiblySchedulableTime)
            {
                offset = MaxPossiblySchedulableTime;
            }

            TimeSpan timeSpan = (offset - _dateTimeProvider.Now);
            if (timeSpan.Ticks < 0)
            {
                offset = _dateTimeProvider.Now;
                timeSpan = TimeSpan.Zero;
            }
            
            var timeSpanMs = (long) timeSpan.TotalMilliseconds;
            
            CurrentlyScheduledTime = offset;
            _timer.Change(timeSpanMs, Timeout.Infinite);
        }

        private void SetIdle()
        {
            CurrentlyScheduledTime = DateTimeOffset.MaxValue;
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        protected virtual void DoWork()
        {
            CurrentlyScheduledTime = DateTimeOffset.MaxValue;
            _action();
        }

        private DateTimeOffset MaxPossiblySchedulableTime
        {
            get
            {
                const int maxDueTimeMs = int.MaxValue - 3; // internal limitation of System.Threading.Timer.
                return DateTimeOffset.Now.AddMilliseconds(maxDueTimeMs);
            }
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}