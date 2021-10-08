using System.Net;
using Microsoft.AspNetCore.Mvc;
using AuthoBson.Shared;
using AuthoBson.Shared.Data.Models;
using AuthoBson.Messaging.Extensions;
using AuthoBson.Messaging.Services;
using AuthoBson.Messaging.Services.Shared;
using AuthoBson.Messaging.Data.Models;
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
        
        // GET api/message
        [HttpGet("api/message/[controller]")]
        [SwaggerResponse((int) HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int) HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public ActionResult<Message> Get(string Id, [Messaging] IModelBase sender, [Messaging(true)] IModelBase receiver)
        {
            return _messageService.getMessage(Id);
            /*return this.FromServiceResult(
                _messageService.getMessage(Id);
            );*/
        }

        // POST api/message
        [HttpPost("api/message/[controller]")]
        [SwaggerResponse((int) HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int) HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public ActionResult<Message> Post(Message Message, [Messaging] IModelBase sender, [Messaging(true)] IModelBase receiver) {
            Message response = _messageService.CreateMessage(Message);

            if (response != null)
                return CreatedAtRoute("GetUser", new { }, Message);

            return Conflict("User scheme is incorrect");
        }
    }
}