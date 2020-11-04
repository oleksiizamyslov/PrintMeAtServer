using System;
using System.Threading.Tasks;
using PrintMeAtServer.Core.Interfaces;
using PrintMeAtServer.Core.Models;

namespace PrintMeAtServer.Core.Impl
{
    public class SimpleMessageProcessor : IMessageProcessor
    {
        public SimpleMessageProcessor()
        {
        }

        public Task Process(Message message)
        {
            Console.WriteLine(message.MessageText);

            return Task.CompletedTask;
        }
    }
}