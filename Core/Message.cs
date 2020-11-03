using System;

namespace Core
{
    public class Message:IComparable<Message>
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

        public int CompareTo(Message other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return DateTime.CompareTo(other.DateTime);
        }
    }
}