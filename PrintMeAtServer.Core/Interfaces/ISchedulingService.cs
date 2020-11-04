using System;
using System.Threading.Tasks;

namespace PrintMeAtServer.Core.Interfaces
{
    public interface ISchedulingService
    {
        Task ScheduleProcessing(DateTimeOffset dateTime);
    }
}