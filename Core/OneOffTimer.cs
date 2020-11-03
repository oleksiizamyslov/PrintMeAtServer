using System;
using System.Threading;

namespace Core
{
    public class OneOffTimer:IDisposable
    {
        private readonly Action _action;
        private readonly Timer _timer;
        
        public OneOffTimer(Action action)
        {
            _action = action;
            _timer = new Timer((st) => DoWork(), null, Timeout.Infinite, Timeout.Infinite);
            SetIdle();
        }

        public DateTimeOffset CurrentScheduledTime
        {
            get => DTO;
            set => DTO = value;
        }
        // CLEAN
        public static DateTimeOffset DTO { get; private set; }

        private DateTimeOffset MaxScheduledTime
        {
            get
            {
                const int maxDueTimeMs = Int32.MaxValue - 3;
                return DateTimeOffset.Now.AddMilliseconds(maxDueTimeMs);
            }
        }

        public void Schedule(DateTimeOffset offset)
        {
            if (offset > MaxScheduledTime)
            {
                offset = MaxScheduledTime;
            }

            TimeSpan timeSpan = (offset - DateTimeOffset.Now);
            if (timeSpan.Ticks < 0)
            {
                offset = DateTimeOffset.Now;
                timeSpan = TimeSpan.Zero;
            }
            
            var timeSpanMs = (long) timeSpan.TotalMilliseconds;
            
            CurrentScheduledTime = offset;
            _timer.Change(timeSpanMs, Timeout.Infinite);
        }

        private void SetIdle()
        {
            CurrentScheduledTime = DateTimeOffset.MaxValue;
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void DoWork()
        {
            CurrentScheduledTime = DateTimeOffset.MaxValue;
            _action();
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}