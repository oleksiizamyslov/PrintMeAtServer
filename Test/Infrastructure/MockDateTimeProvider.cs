using System;
using Core.Interfaces;

namespace Test.Infrastructure
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