using System;
using Core;

namespace Test
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