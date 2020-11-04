using System.Threading.Tasks;
using Core.Data;
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

        public async Task<ActionResult<string>> PrintMe([FromQuery] Message message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _printMeService.EnqueueMessage(message);

            return Ok();
        }
    }
}
