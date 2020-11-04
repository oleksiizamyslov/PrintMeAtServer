using System;

namespace Core.Models
{
    public class Message
    {
        // ReSharper disable once UnusedMember.Global - For Json Deserialization
        public Message()
        {
        }

        public Message(DateTimeOffset dateTime, string messageText)
        {
            DateTime = dateTime;
            MessageText = messageText;
        }

        public DateTimeOffset DateTime { get; set; }

        public string MessageText { get; set; }
    }
}