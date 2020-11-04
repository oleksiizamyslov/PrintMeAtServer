using System;
using PrintMeAtServer.Core.Interfaces;

namespace PrintMeAtServer.Test.Infrastructure
{
    public class MockDateTimeProvider : IDateTimeProvider
    {
        public MockDateTimeProvider(DateTimeOffset dto)
        {
            Now = dto;
        }

        public DateTimeOffset Now { get; set; }
    }
}