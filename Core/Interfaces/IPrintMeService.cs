using System;
using System.Threading.Tasks;
using Core;

namespace PrintMeAtServer
{
    public class MessageProcessor : IMessageProcessor
    {
        private IDateTimeProvider _dateTimeProvider;

        public static string Str = "";

        public MessageProcessor(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public Task Process(Message message)
        {
            var diff = (_dateTimeProvider.Now - message.DateTime).TotalMilliseconds;
            var text = $"{diff}: {message.Text} of {message.DateTime} printed at {_dateTimeProvider.Now}";
            Str = text + "\r\n" + Str;
            Console.WriteLine(message.Text);

            return Task.CompletedTask;
        }
    }

}