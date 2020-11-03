using System;
using System.Threading.Tasks;
using Core;

namespace PrintMeAtServer
{
    public interface IPrintMeService
    {
        Task AddMessage(Message newMessage);
        Task Initialize();
    }

    public class MessageProcessor : IMessageProcessor
    {
        public static string Str = "";

        public Task Process(Message message)
        {
            var diff = (DateTimeOffset.Now - message.DateTime).TotalMilliseconds;
            var text = $"{diff}: {message.Text} of {message.DateTime} printed at {DateTimeOffset.Now}";
            Str = text + "\r\n" + Str;
            Console.WriteLine(message.Text);

            return Task.CompletedTask;
        }
    }

}