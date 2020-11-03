using System;

namespace Core
{
    public class Message
    {
        public Message()
        {
        }

        public Message(DateTimeOffset dateTime, string text)
        {
            DateTime = dateTime;
            Text = text;
        }

        public DateTimeOffset DateTime { get; set; }
        public string Text { get; set; }
    }
}