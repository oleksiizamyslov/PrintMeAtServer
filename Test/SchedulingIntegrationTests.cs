using System;
using Core;
using Core.Interfaces;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using PrintMeAtServer;
using Test.Infrastructure;

namespace Test
{
    public class SchedulingIntegrationTests
    {
        private static readonly DateTimeOffset _offsetBefore = DateTimeOffset.Now;
        private static readonly DateTimeOffset _offset1 = _offsetBefore.AddMilliseconds(10);
        private static readonly DateTimeOffset _offset2 = _offset1.AddMilliseconds(10);
        private static readonly DateTimeOffset _offsetAfter = _offset2.AddMilliseconds(10);

        private static readonly Message _message1 = new Message(_offset1, "M1");
        private static readonly Message _message2 = new Message(_offset2, "M2");
        
        private Mock<IMessageProcessor>  _messageProcessor;
        private SchedulingService _schedulingService;
        private int _processedMessages;

        private readonly IResolveConstraint _pollingInterval = Is.True.After(3000).PollEvery(50);

        private IPrintMeAtService InitializeService(IMessageQueue messageQueue, IDateTimeProvider dto)
        {
            _schedulingService = new SchedulingService(messageQueue, _messageProcessor.Object, dto);
            _schedulingService.Initialize().GetAwaiter().GetResult();
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
        public void Should_Immediately_Print_Outdated_Messages_When_Starting_Up()
        {
            TestMessageQueue mq = new TestMessageQueue();

            mq.EnqueueMessage(_message1);
            mq.EnqueueMessage(_message2);
            
            InitializeService(mq, new MockDateTimeProvider(_offsetAfter));

            Assert.That(() => _processedMessages == 2, _pollingInterval);

            _messageProcessor.Verify((p) => p.Process(_message1), Times.Once);
            _messageProcessor.Verify((p) => p.Process(_message2), Times.Once);
            _messageProcessor.VerifyNoOtherCalls();
        }

        [Test]
        public void Should_Schedule_And_Print_Messages_In_Batches()
        {
            TestMessageQueue mq = new TestMessageQueue();

            var mockProvider = new MockDateTimeProvider(_offsetBefore);

            var service = InitializeService(mq, mockProvider);

            service.EnqueueMessage(_message1);
            service.EnqueueMessage(_message2);
            
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
        public void Should_Schedule_And_Print_Messages_As_Scheduled_Regardless_Of_Adding_Order()
        {
            TestMessageQueue mq = new TestMessageQueue();

            var mockProvider = new MockDateTimeProvider(_offsetBefore);

            var service = InitializeService(mq, mockProvider);

            service.EnqueueMessage(_message2);
            service.EnqueueMessage(_message1);

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
        public void Should_Schedule_And_Print_Messages_One_By_One()
        {
            TestMessageQueue mq = new TestMessageQueue();

            var mockProvider = new MockDateTimeProvider(_offsetBefore);

            var service = InitializeService(mq, mockProvider);

            service.EnqueueMessage(_message1);
            _messageProcessor.Verify((p) => p.Process(It.IsAny<Message>()), Times.Never);
            mockProvider.Now = _offset1;

            Assert.That(() => _processedMessages == 1, _pollingInterval);
            _messageProcessor.Verify((p) => p.Process(_message1), Times.Once);
            
            service.EnqueueMessage(_message2);
            mockProvider.Now = _offset2;
            Assert.That(() => _processedMessages == 2, _pollingInterval);
            _messageProcessor.Verify((p) => p.Process(_message2), Times.Once);
            _messageProcessor.VerifyNoOtherCalls();
        }

        [Test]
        public void Should_Immediately_Print_Messages_In_The_Past()
        {
            TestMessageQueue mq = new TestMessageQueue();

            var mockProvider = new MockDateTimeProvider(_offsetAfter);

            var service = InitializeService(mq, mockProvider);

            service.EnqueueMessage(_message1);
            service.EnqueueMessage(_message2);

            Assert.That(() => _processedMessages == 2, _pollingInterval);
            _messageProcessor.Verify((p) => p.Process(_message1), Times.Once);
            _messageProcessor.Verify((p) => p.Process(_message2), Times.Once);
            _messageProcessor.VerifyNoOtherCalls();
        }

        [Test]
        public void Should_Schedule_And_Print_Messages_Scheduled_For_Same_Time()
        {
            TestMessageQueue mq = new TestMessageQueue();

            var mockProvider = new MockDateTimeProvider(_offsetBefore);

            var service = InitializeService(mq, mockProvider);

            var sameDateMessage = new Message(_message1.DateTime, "New message");
            service.EnqueueMessage(_message1);
            service.EnqueueMessage(sameDateMessage);
            
            mockProvider.Now = _offsetAfter;

            Assert.That(() => _processedMessages == 2, _pollingInterval);
            _messageProcessor.Verify((p) => p.Process(_message1), Times.Once);
            _messageProcessor.Verify((p) => p.Process(sameDateMessage), Times.Once);
            _messageProcessor.VerifyNoOtherCalls();
        }
    }
}