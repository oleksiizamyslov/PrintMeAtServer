using System;
using Core.Impl;
using NUnit.Framework;
using Test.Infrastructure;

namespace Test
{
    [TestFixture]
    public class OneOffTimerTest
    {
        [Test]
        public void Should_Allow_Scheduling_Time_In_Far_Future()
        {
            OneOffTimer ot = new OneOffTimer(() => {}, new DateTimeProvider());
            ot.Reschedule(DateTimeOffset.Now.AddYears(10));
            var scheduledTime = ot.CurrentlyScheduledTime;
            var scheduledIntervalDays = (scheduledTime - DateTimeOffset.Now).TotalDays;
            var expectedScheduledIntervalDays = TimeSpan.FromMilliseconds(int.MaxValue).TotalDays;
            Assert.IsTrue(Math.Abs(expectedScheduledIntervalDays-scheduledIntervalDays)<0.01);
        }

        [Test]
        public void Should_Allow_Schedule_For_Current_Time_For_Dates_In_The_Past()
        {
            var now = DateTimeOffset.Now;
            OneOffTimer ot = new OneOffTimer(() => { }, new MockDateTimeProvider(now));
            ot.Reschedule(DateTimeOffset.Now.AddYears(-10));
            var scheduledTime = ot.CurrentlyScheduledTime;
            Assert.AreEqual(now, scheduledTime);
        }
    }
}