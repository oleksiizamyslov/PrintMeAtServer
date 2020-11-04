using System;
using System.Threading.Tasks;
using Core;
using Core.Data;
using Core.Impl;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PrintMeAtServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrintMeController : ControllerBase
    {
        private readonly IPrintMeAtService _printMeService;

        public PrintMeController(IPrintMeAtService printMeService)
        {
            _printMeService = printMeService;
        }

        public async Task<ActionResult<string>> PrintMe(DateTimeOffset dateTime, string message)
        {
            var newMessage = new Message(dateTime, message);
            if (message != null)
            {
                await _printMeService.EnqueueMessage(newMessage);
            }

            return $"Scheduled at \r\n {SimpleMessageProcessor.Str}";
        }
    }
}
