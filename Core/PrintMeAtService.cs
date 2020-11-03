using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces;
using PrintMeAtServer;

namespace Core
{
    public interface ISchedulingService
    {
        Task ScheduleProcessing(DateTimeOffset dateTime);
        Task Initialize();
    }

    public class SchedulingService:ISchedulingService, IDisposable
    {
        private const int LOCK_TIMEOUT = 1000;

        private readonly IMessageQueue _messageQueue;
        private readonly IMessageProcessor _messageProcessor;
        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly OneOffTimer _timer;
        private readonly SemaphoreSlim _queueHeadLock = new SemaphoreSlim(1);
        
        public SchedulingService(IMessageQueue messageQueue, IMessageProcessor messageProcessor, IDateTimeProvider dateTimeProvider)
        {
            _messageQueue = messageQueue;
            _messageProcessor = messageProcessor;
            _dateTimeProvider = dateTimeProvider;
            _timer = new OneOffTimer(ProcessNextMessage, dateTimeProvider);
        }

        public async Task Initialize()
        {
            try
            {
                await _queueHeadLock.WaitAsync(LOCK_TIMEOUT);
                await ScheduleNextPendingMessage();
            }
            finally
            {
                _queueHeadLock.Release();
            }
        }

        public async Task ScheduleProcessing(DateTimeOffset newMessageOffset)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            if (newMessageOffset < _timer.CurrentlyScheduledTime)
            {
                try
                {
                    await _queueHeadLock.WaitAsync(LOCK_TIMEOUT);
                    if (newMessageOffset < _timer.CurrentlyScheduledTime)
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
                await _queueHeadLock.WaitAsync(LOCK_TIMEOUT);

                var message = await _messageQueue.PeekNextScheduledMessage();
                if (message != null)
                {
                    if (message.DateTime > _dateTimeProvider.Now)
                    {
                        // Reschedule. Happens in case of dates far in the future.
                        await ScheduleProcessing(message.DateTime);
                        return;
                    }

                    message = await _messageQueue.DequeueNextScheduledMessage();
                    await _messageProcessor.Process(message);
                }

                await ScheduleNextPendingMessage();
            }
            finally
            {
                _queueHeadLock.Release();
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

    public class PrintMeAtService : IPrintMeAtService
    {
        private readonly IMessageQueue _messageQueue;
        private readonly ISchedulingService _schedulingService;
        
        public PrintMeAtService(IMessageQueue messageQueue, ISchedulingService schedulingService)
        {
            _messageQueue = messageQueue;
            _schedulingService = schedulingService;
        }

        public async Task EnqueueMessage(Message message)
        {
            await _messageQueue.EnqueueMessage(message);
            await _schedulingService.ScheduleProcessing(message.DateTime);
        }
    }
}