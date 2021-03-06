﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PrintMeAtServer.API.Models;
using PrintMeAtServer.Core.Interfaces;
using PrintMeAtServer.Core.Models;

namespace PrintMeAtServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrintMeAtController : ControllerBase
    {
        private readonly IPrintMeAtService _printMeService;
        public PrintMeAtController(IPrintMeAtService printMeService)
        {
            _printMeService = printMeService;
        }

        public async Task<ActionResult<string>> PrintMe([FromQuery] MessageModel messageModel)
        {
            await _printMeService.EnqueueMessage(new Message(messageModel.DateTime.Value, messageModel.MessageText));

            return Ok("Message queued successfully!");
        }
    }
}
