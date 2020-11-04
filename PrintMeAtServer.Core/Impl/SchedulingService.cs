using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PrintMeAtServer.Core.Interfaces;

namespace PrintMeAtServer.Core.Impl
{
    /// <summary>
    /// Enables messaging scheduling by continuously scheduling processing for the current message queue head.
    /// </summary>
    public class SchedulingService : ISchedulingService, IDisposable
    {
        private const int LOCK_TIMEOUT = 1000;

        private readonly IMessageQueue _messageQueue;
        private readonly IMessageProcessor _messageProcessor;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger<ISchedulingService> _logger;
        private readonly IOneOffTimer _timer;

        private readonly SemaphoreSlim _queueHeadScheduleLock = new SemaphoreSlim(1);

        public SchedulingService(IMessageQueue messageQueue, 
            IMessageProcessor messageProcessor, 
            IDateTimeProvider dateTimeProvider, 
            ITimerFactory timerFactory, 
            ILogger<ISchedulingService> logger)
        {
            _messageQueue = messageQueue;
            _messageProcessor = messageProcessor;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
            _timer = timerFactory.Create(ProcessNextMessage);
        }

        public async Task Initialize()
        {
            try
            {
                await _queueHeadScheduleLock.WaitAsync(LOCK_TIMEOUT);
                await ScheduleNextPendingMessage();
            }
            finally
            {
                _queueHeadScheduleLock.Release();
            }
        }

        public async Task ScheduleProcessing(DateTimeOffset newMessageOffset)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            if (newMessageOffset < _timer.CurrentlyScheduledTime)
            {
                try
                {
                    await _queueHeadScheduleLock.WaitAsync(LOCK_TIMEOUT);
                    if (newMessageOffset < _timer.CurrentlyScheduledTime)
                    {
                        _logger.LogDebug($"Next scheduled processing changed to {newMessageOffset}");
                        _timer.Reschedule(newMessageOffset);
                    }
                }
                finally
                {
                    _queueHeadScheduleLock.Release();
                }
            }
        }

        private async Task ScheduleNextPendingMessage()
        {
            var nextMessage = await _messageQueue.PeekNextScheduledMessage();
            if (nextMessage != null)
            {
                await ScheduleProcessing(nextMessage.DateTime);
            }
        }

        private async void ProcessNextMessage()
        {
            try
            {
                await _queueHeadScheduleLock.WaitAsync(LOCK_TIMEOUT);

                var message = await _messageQueue.PeekNextScheduledMessage();
                if (message != null)
                {
                    if (message.DateTime > _dateTimeProvider.Now)
                    {
                        // Reschedule. Happens in case of dates far in the future.
                        _logger.LogDebug($"Rescheduling {message}");
                        await ScheduleProcessing(message.DateTime);
                        return;
                    }

                    message = await _messageQueue.DequeueNextScheduledMessage();
                    _logger.LogDebug($"Processing {message}");
                    await _messageProcessor.Process(message);
                }

                await ScheduleNextPendingMessage();
            }
            finally
            {
                _queueHeadScheduleLock.Release();
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}