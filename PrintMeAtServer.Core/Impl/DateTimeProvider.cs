using System;
using PrintMeAtServer.Core.Interfaces;

namespace PrintMeAtServer.Core.Impl
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset Now => DateTimeOffset.Now;
    }
}