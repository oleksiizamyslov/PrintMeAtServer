using System;
using System.Threading.Tasks;
using Core.Data;
using Core.Interfaces;

namespace Core.Impl
{
    public class SimpleMessageProcessor : IMessageProcessor
    {
        public SimpleMessageProcessor()
        {
        }

        public Task Process(Message message)
        {
            Console.WriteLine(message.Text);

            return Task.CompletedTask;
        }
    }
}