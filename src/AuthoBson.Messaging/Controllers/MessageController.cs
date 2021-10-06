using System.Net;
using Microsoft.AspNetCore.Mvc;
using AuthoBson;
using AuthoBson.Messaging.Extensions;
using AuthoBson.Messaging.Services;
using AuthoBson.Messaging.Services.Shared;
using Swashbuckle.AspNetCore.Annotations;

namespace AuthoBson.Messaging.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly MessageService _messageService;

        public MessageController(MessageService messageService)
        {
            _messageService = messageService;
        }
        
        
        // GET api/home
        [HttpGet("api/message/[controller]")]
        [SwaggerResponse((int) HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int) HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public IActionResult GetMessage(object source, object sender, object receiver)
        {
            return this.FromServiceResult(
                _messageService.GetMessage()
            );
        }

        [HttpPost("api/message/[controller]")]
        public ActionResult PostMessage(object source, object receiver) {
        }
    }
}