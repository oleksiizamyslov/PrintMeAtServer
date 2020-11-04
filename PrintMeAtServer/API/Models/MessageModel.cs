using System;
using System.ComponentModel.DataAnnotations;

namespace PrintMeAtServer.API.Models
{
    public class MessageModel
    {
        // ReSharper disable once UnusedMember.Global - For Json Deserialization
        public MessageModel()
        {
        }

        public MessageModel(DateTimeOffset dateTime, string messageText)
        {
            DateTime = dateTime;
            MessageText = messageText;
        }

        [Required]
        public DateTimeOffset? DateTime { get; set; }

        [Required]
        public string MessageText { get; set; }
    }
}