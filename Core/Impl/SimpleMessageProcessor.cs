using System;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models;

namespace Core.Impl
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