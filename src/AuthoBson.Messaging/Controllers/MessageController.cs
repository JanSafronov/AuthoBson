using System.Collections;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using AuthoBson.Shared;
using AuthoBson.Shared.Results;
using AuthoBson.Shared.Data.Models;
using AuthoBson.Messaging.Extensions;
using AuthoBson.Messaging.Services;
using AuthoBson.Messaging.Data.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace AuthoBson.Messaging.Controllers
{
    [Route("api/message", Name = "message")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Messaging")]
    public class MessageController : ControllerBase
    {
        private readonly MessageService _messageService;

        public MessageController(MessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("{senderId:length(24)}/{receiverId:length(24)}", Name = "GetMessages")]
        [SwaggerResponse((int) HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int) HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public ActionResult<List<Message>> Get(string senderId = null, string receiverId = null)
        {
            return _messageService.GetAll(senderId, receiverId);
        }
        
        [HttpGet("{Id:length(24)}", Name = "GetMessage")]
        [SwaggerResponse((int) HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int) HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public ActionResult<Message> Get(string Id)
        {
            return _messageService.GetMessage(Id);
        }

        [HttpPost(Name = "CreateMessage")]
        [SwaggerResponse((int) HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int) HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public ActionResult<Message> Create(Message Message) {//, [Messaging] IModelBase sender, [Messaging(true)] IModelBase receiver) {
            Message response = _messageService.CreateMessage(Message);


            if (response != null)
                return CreatedAtRoute("CreateMessage", new { id = Message.Id.ToString() }, Message);

            return Conflict("Message scheme is incorrect");
        }
    }
}
