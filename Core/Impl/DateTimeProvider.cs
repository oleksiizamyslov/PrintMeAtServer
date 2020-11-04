using System;
using Core.Interfaces;

namespace Core.Impl
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset Now => DateTimeOffset.Now;
    }
}