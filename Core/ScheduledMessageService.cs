using System;
using System.Threading;
using System.Threading.Tasks;
using PrintMeAtServer;

namespace Core
{
    public class ScheduledMessageService : IScheduledMessageService
    {
        private readonly IMessageProcessor _messageProcessor;
        private readonly IPersistentMessageQueue _messageQueue;
        private readonly OneOffTimer _timer;
        private readonly SemaphoreSlim _queueHeadLock = new SemaphoreSlim(1);

        public ScheduledMessageService(IMessageProcessor messageProcessor, IPersistentMessageQueue messageQueue)
        {
            _messageQueue = messageQueue;
            _messageProcessor = messageProcessor;
            _timer = new OneOffTimer(ProcessNextMessage);
        }

        public async Task EnqueueMessage(Message message)
        {
            await _messageQueue.EnqueueMessage(message);

            await RefreshNextMessageProcessingSchedule(message.DateTime);
        }

        public async Task Initialize()
        {
            try
            {
                await _queueHeadLock.WaitAsync(1000);

                await ScheduleNextPendingMessage();
            }
            finally
            {
                _queueHeadLock.Release();
            }
        }

        private async void ProcessNextMessage()
        {
            try
            {
                await _queueHeadLock.WaitAsync(1000);

                var message = await _messageQueue.PeekNextScheduledMessage();
                if (message.DateTime >= DateTimeOffset.Now)
                {
                    // Reschedule.
                    await RefreshNextMessageProcessingSchedule(message.DateTime);
                    return;
                }
                
                message = await _messageQueue.PopNextScheduledMessage();
                await _messageProcessor.Process(message);

                await ScheduleNextPendingMessage();
            }
            finally
            {
                _queueHeadLock.Release();
            }
        }

        private async Task ScheduleNextPendingMessage()
        {
            var nextMessage = await _messageQueue.PeekNextScheduledMessage();
            if (nextMessage != null)
            {
                await RefreshNextMessageProcessingSchedule(nextMessage.DateTime);
            }
        }

        private async Task RefreshNextMessageProcessingSchedule(DateTimeOffset newMessageOffset)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            if (newMessageOffset < _timer.CurrentScheduledTime)
            {
                try
                {
                    await _queueHeadLock.WaitAsync(1000);
                    if (newMessageOffset < _timer.CurrentScheduledTime)
                    {
                        _timer.Schedule(newMessageOffset);
                    }
                }
                finally
                {
                    _queueHeadLock.Release();
                }
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}