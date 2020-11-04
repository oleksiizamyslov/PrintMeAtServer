using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Data
{
    public class Message
    {
        // ReSharper disable once UnusedMember.Global - For Json Deserialization
        public Message()
        {
        }

        public Message(DateTimeOffset dateTime, string text)
        {
            DateTime = dateTime;
            Text = text;
        }

        [Required]
        public DateTimeOffset DateTime { get; set; }

        [Required]
        public string Text { get; set; }
    }
}