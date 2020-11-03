using System;
using System.Threading.Tasks;
using Core;
using Microsoft.AspNetCore.Mvc;

namespace PrintMeAtServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrintMeController : ControllerBase
    {
        private readonly IPrintMeService _printMeService;

        public PrintMeController(IPrintMeService printMeService)
        {
            _printMeService = printMeService;
        }

        public async Task<ActionResult<string>> PrintMe(DateTimeOffset dateTime, string message)
        {
            var newMessage = new Message(dateTime, message);
            if (message != null)
            {
                await _printMeService.AddMessage(newMessage);
            }

            return $"Scheduled at {OneOffTimer.DTO} \r\n {MessageProcessor.Str}";
        }
    }
}
