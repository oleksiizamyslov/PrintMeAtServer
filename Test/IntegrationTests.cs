using System;
using System.Threading.Tasks;
using Core.Data;
using Core.Impl;
using Core.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Test.Infrastructure;

namespace Test
{
    public class IntegrationTests
    {
        private static readonly DateTimeOffset _offsetBefore = DateTimeOffset.Now;
        private static readonly DateTimeOffset _offset1 = _offsetBefore.AddMilliseconds(10);
        private static readonly DateTimeOffset _offset2 = _offset1.AddMilliseconds(10);
        private static readonly DateTimeOffset _offsetAfter = _offset2.AddMilliseconds(10);

        private static readonly Message _message1 = new Message(_offset1, "M1");
        private static readonly Message _message2 = new Message(_offset2, "M2");

        private Mock<IMessageProcessor> _messageProcessor;
        private SchedulingService _schedulingService;
        private int _processedMessages;

        private readonly IResolveConstraint _pollingInterval = Is.True.After(3000).PollEvery(50);

        private async Task<IPrintMeAtService> InitializeService(IMessageQueue messageQueue, IDateTimeProvider dto)
        {
            _schedulingService = new SchedulingService(messageQueue, _messageProcessor.Object, dto, new TimerFactory(dto), new Mock<ILogger<ISchedulingService>>().Object);
            await _schedulingService.Initialize();
            return new PrintMeAtService(messageQueue, _schedulingService);
        }

        [SetUp]
        public void SetUp()
        {
            _messageProcessor = new Mock<IMessageProcessor>();
            _messageProcessor.Setup(p => p.Process(It.IsAny<Message>()))
                .Callback<Message>((m) => _processedMessages++);
            _processedMessages = 0;
        }

        [TearDown]
        public void TearDown()
        {
            _schedulingService?.Dispose();
        }

        [Test]
        public async Task Should_Immediately_Print_Outdated_Messages_When_Starting_Up()
        {
            InMemoryMessageQueue mq = new InMemoryMessageQueue();

            await mq.EnqueueMessage(_message1);
            await mq.EnqueueMessage(_message2);

            await InitializeService(mq, new MockDateTimeProvider(_offsetAfter));

            Assert.That(() => _processedMessages == 2, _pollingInterval);

            _messageProcessor.Verify((p) => p.Process(_message1), Times.Once);
            _messageProcessor.Verify((p) => p.Process(_message2), Times.Once);
            _messageProcessor.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Should_Schedule_And_Print_Messages_In_Batches()
        {
            InMemoryMessageQueue mq = new InMemoryMessageQueue();

            var mockProvider = new MockDateTimeProvider(_offsetBefore);

            var service = await InitializeService(mq, mockProvider);

            await service.EnqueueMessage(_message1);
            await service.EnqueueMessage(_message2);

            _messageProcessor.Verify((p) => p.Process(It.IsAny<Message>()), Times.Never);

            mockProvider.Now = _offset1;
            Assert.That(() => _processedMessages == 1, _pollingInterval);
            _messageProcessor.Verify((p) => p.Process(_message1), Times.Once);

            mockProvider.Now = _offset2;
            Assert.That(() => _processedMessages == 2, _pollingInterval);
            _messageProcessor.Verify((p) => p.Process(_message2), Times.Once);
            _messageProcessor.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Should_Schedule_And_Print_Messages_As_Scheduled_Regardless_Of_Adding_Order()
        {
            InMemoryMessageQueue mq = new InMemoryMessageQueue();

            var mockProvider = new MockDateTimeProvider(_offsetBefore);

            var service = await InitializeService(mq, mockProvider);

            await service.EnqueueMessage(_message2);
            await service.EnqueueMessage(_message1);

            _messageProcessor.Verify((p) => p.Process(It.IsAny<Message>()), Times.Never);
            mockProvider.Now = _offset1;
            Assert.That(() => _processedMessages == 1, _pollingInterval);
            _messageProcessor.Verify((p) => p.Process(_message1), Times.Once);

            mockProvider.Now = _offset2;
            Assert.That(() => _processedMessages == 2, _pollingInterval);
            _messageProcessor.Verify((p) => p.Process(_message2), Times.Once);
            _messageProcessor.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Should_Schedule_And_Print_Messages_One_By_One()
        {
            InMemoryMessageQueue mq = new InMemoryMessageQueue();

            var mockProvider = new MockDateTimeProvider(_offsetBefore);

            var service = await InitializeService(mq, mockProvider);

            await service.EnqueueMessage(_message1);
            _messageProcessor.Verify((p) => p.Process(It.IsAny<Message>()), Times.Never);
            mockProvider.Now = _offset1;

            Assert.That(() => _processedMessages == 1, _pollingInterval);
            _messageProcessor.Verify((p) => p.Process(_message1), Times.Once);

            await service.EnqueueMessage(_message2);
            mockProvider.Now = _offset2;
            Assert.That(() => _processedMessages == 2, _pollingInterval);
            _messageProcessor.Verify((p) => p.Process(_message2), Times.Once);
            _messageProcessor.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Should_Immediately_Print_Messages_In_The_Past()
        {
            InMemoryMessageQueue mq = new InMemoryMessageQueue();

            var mockProvider = new MockDateTimeProvider(_offsetAfter);

            var service = await InitializeService(mq, mockProvider);

            await service.EnqueueMessage(_message1);
            await service.EnqueueMessage(_message2);

            Assert.That(() => _processedMessages == 2, _pollingInterval);
            _messageProcessor.Verify((p) => p.Process(_message1), Times.Once);
            _messageProcessor.Verify((p) => p.Process(_message2), Times.Once);
            _messageProcessor.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Should_Schedule_And_Print_Messages_Scheduled_For_Same_Time()
        {
            InMemoryMessageQueue mq = new InMemoryMessageQueue();

            var mockProvider = new MockDateTimeProvider(_offsetBefore);

            var service = await InitializeService(mq, mockProvider);

            var sameDateMessage = new Message(_message1.DateTime, "New message");
            await service.EnqueueMessage(_message1);
            await service.EnqueueMessage(sameDateMessage);

            mockProvider.Now = _offsetAfter;

            Assert.That(() => _processedMessages == 2, _pollingInterval);
            _messageProcessor.Verify((p) => p.Process(_message1), Times.Once);
            _messageProcessor.Verify((p) => p.Process(sameDateMessage), Times.Once);
            _messageProcessor.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Should_Reschedule_When_Front_Of_The_Queue_Is_Found_To_Be_In_The_Future_On_Processing()
        {
            InMemoryMessageQueue mq = new InMemoryMessageQueue();

            var mockProvider = new MockDateTimeProvider(_offsetBefore);

            var service = await InitializeService(mq, mockProvider);

            var farFutureMessage = new Message(DateTimeOffset.Now.AddYears(1), "Far future message");
            await service.EnqueueMessage(farFutureMessage);

            // Trigger processing immediately
            await _schedulingService.ScheduleProcessing(DateTimeOffset.Now);

            Assert.AreEqual(_processedMessages, 0);

        }
    }
}