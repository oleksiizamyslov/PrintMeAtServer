using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Core.Impl;
using Core.Interfaces;

namespace Test.Infrastructure
{
    public class SyncTimer:OneOffTimer
    {
        public SyncTimer(Action action, IDateTimeProvider dateTimeProvider) : base(action, dateTimeProvider)
        {
        }

        private readonly ManualResetEvent _manualResetEvent = new ManualResetEvent(false);

        public override void Reschedule(DateTimeOffset offset)
        {
            _manualResetEvent.Reset();
            base.Reschedule(offset);
        }

        protected override void DoWork()
        {
            base.DoWork();
            _manualResetEvent.Set();
        }

        public void WaitTillTriggered()
        {
            _manualResetEvent.WaitOne(1000);
        }
    }

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
