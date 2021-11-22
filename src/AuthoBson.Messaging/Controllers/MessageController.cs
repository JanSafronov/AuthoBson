using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using AuthoBson.Shared;
using AuthoBson.Shared.Services;
using AuthoBson.Shared.Data.Models;
using AuthoBson.Messaging.Extensions;
using AuthoBson.Messaging.Services;
using AuthoBson.Messaging.Data.Models;
using Swashbuckle.AspNetCore.Annotations;
using AuthoBson.Shared.Services.Results;

namespace AuthoBson.Messaging.Controllers
{
    [Route("api/message", Name = "message")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly MessageService _messageService;

        public MessageController(MessageService messageService) =>
            _messageService = messageService;

        [HttpGet("{senderId:length(24)}/{receiverId:length(24)}", Name = "GetMessages")]
        [SwaggerResponse((int) HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int) HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public ActionResult<List<Message>> Get(string senderId = null, string receiverId = null) =>
            _messageService.GetAll(senderId, receiverId);

        [HttpGet("{Id:length(24)}", Name = "GetMessage")]
        [SwaggerResponse((int) HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int) HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public ActionResult<Message> Get(string Id) =>
            _messageService.GetMessage(Id);

        [HttpGet("{Id:length(24)}/async", Name = "GetMessageAsync")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public async Task<ActionResult<Message>> GetAsync(string Id) =>
            await _messageService.GetMessageAsync(Id);

        [HttpPost(Name = "CreateMessage")]
        [SwaggerResponse((int) HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int) HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public ActionResult<Message> Create(Message Message) {
            bool routeAccepted = _messageService.VerifyReferences(Message.Receiver, Message.Sender);
            if (!routeAccepted)
                return new ConflictResult();
            Message response = _messageService.CreateMessage(Message);

            if (response != null)
                return CreatedAtRoute("CreateMessage", new { id = Message.Id.ToString() }, Message);
            
            
            return Conflict("Message scheme is incorrect");
        }

        [HttpPost(Name = "CreateMessageAsync")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public async Task<ActionResult<Message>> CreateAsync(Message Message)
        {
            bool routeAccepted = await _messageService.VerifyReferencesAsync(Message.Receiver, Message.Sender);
            if (!routeAccepted)
                return new ConflictResult();
            Message response = await _messageService.CreateMessageAsync(Message);

            if (response != null)
                return CreatedAtRoute("CreateMessage", new { id = Message.Id.ToString() }, Message);


            return Conflict("Message scheme is incorrect");
        }
    }
}
