using System;
using System.Threading.Tasks;
using Core.Data;
using Core.Impl;
using Core.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class SchedulingServiceTest
    {
        private Mock<IOneOffTimer> _timer;
        private ITimerFactory _timerFactory;

        [SetUp]
        public void SetUp()
        {
            var actualTimer = new OneOffTimer(() => { /* do nothing */ }, new DateTimeProvider());
            _timer = new Mock<IOneOffTimer>();
            _timer.SetupGet(p => p.CurrentlyScheduledTime)
               .Returns(() => actualTimer.CurrentlyScheduledTime);
            _timer.Setup(p => p.Reschedule(It.IsAny<DateTimeOffset>()))
                .Callback<DateTimeOffset>(dto => actualTimer.Reschedule(dto));

            var timerFactory = new Mock<ITimerFactory>();
            timerFactory.Setup(p => p.Create(It.IsAny<Action>())).Returns(_timer.Object);
            _timerFactory = timerFactory.Object;
        }

        [Test]
        public async Task Should_Schedule_Nothing_When_Initializing_With_Empty_Queue()
        {
            var mq = new Mock<IMessageQueue>();

            SchedulingService ss = new SchedulingService(mq.Object, new SimpleMessageProcessor(),
                new DateTimeProvider(), _timerFactory, new Mock<ILogger<ISchedulingService>>().Object);

            await ss.Initialize();

            _timer.Verify(t => t.Reschedule(It.IsAny<DateTimeOffset>()), Times.Never);
        }

        [Test]
        public async Task Should_Schedule_Existing_Message_When_Initializing_With_Non_Empty_Queue()
        {
            var mq = new Mock<IMessageQueue>();

            var dto = DateTimeOffset.Now;
            mq.Setup(p => p.PeekNextScheduledMessage()).ReturnsAsync(new Message(dto, "test"));

            SchedulingService ss = new SchedulingService(mq.Object, new SimpleMessageProcessor(),
                new DateTimeProvider(), _timerFactory, new Mock<ILogger<ISchedulingService>>().Object);

            await ss.Initialize();

            _timer.Verify(t => t.Reschedule(dto));
        }

        [Test]
        public async Task Should_Reschedule_For_New_Front_Of_The_Queue()
        {
            var dto = DateTimeOffset.Now.AddDays(1);
            var dtoBefore = DateTimeOffset.Now;
            var mq = new Mock<IMessageQueue>();
            mq.Setup(p => p.PeekNextScheduledMessage()).ReturnsAsync(new Message(dto, "test"));

            SchedulingService ss = new SchedulingService(mq.Object, new SimpleMessageProcessor(),
                new DateTimeProvider(), _timerFactory, new Mock<ILogger<ISchedulingService>>().Object);
            await ss.Initialize();

            await ss.ScheduleProcessing(dtoBefore);

            _timer.Verify(t => t.Reschedule(dto), Times.Once);
            _timer.Verify(t => t.Reschedule(dtoBefore), Times.Once);
        }

        [Test]
        public async Task Should_Not_Reschedule_For_Message_That_Is_Not_In_Front_Of_The_Queue()
        {
            var dto = DateTimeOffset.Now.AddDays(1);
            var dtoLater = dto.AddDays(1);
            var mq = new Mock<IMessageQueue>();
            mq.Setup(p => p.PeekNextScheduledMessage()).ReturnsAsync(new Message(dto, "test"));

            SchedulingService ss = new SchedulingService(mq.Object, new SimpleMessageProcessor(),
                new DateTimeProvider(), _timerFactory, new Mock<ILogger<ISchedulingService>>().Object);
            await ss.Initialize();

            await ss.ScheduleProcessing(dtoLater);

            _timer.Verify(t => t.Reschedule(dto), Times.Once);
            _timer.Verify(t => t.Reschedule(dtoLater), Times.Never);
        }
    }
}
