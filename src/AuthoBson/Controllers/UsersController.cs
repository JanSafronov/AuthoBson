﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Transactions;
using System.Security.Policy;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using AuthoBson.Models;
using AuthoBson.Services;
using AuthoBson.Email;
using AuthoBson.Email.Settings;
using AuthoBson.Shared.Results;
using Swashbuckle.AspNetCore.Annotations;

namespace AuthoBson.Controllers {

    [ApiController]
    [Route("api/user", Name = "User")]
    public class UserController : ControllerBase {

        private readonly UserService _userService;

        private readonly MailSender _mailSender;

        public string[] templates { get; set; }

        [ActivatorUtilitiesConstructor]
        public UserController(UserService userService, IDomainSettings mailSettings)
        {
            _userService = userService;
            _mailSender = new SMTPMail(mailSettings);
        }

        public UserController(UserService userService)
        {
            _userService = userService;
            _mailSender = null;
        }

        [HttpGet(Name = "GetUsers")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public ActionResult<List<User>> Get() =>
            _userService.GetAll();
        
        [HttpGet("{id:length(24)}", Name = "GetUser")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public ActionResult<User> Get(string id) {
            User user = _userService.GetUser(id);

            if (user == null) {
                return NotFound(user);
            }
            
            return new ObjectResult(user);
        }

        [HttpPost(Name = "CreateUser")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public IActionResult Create(User user)
        {
            user.Suspension = new Suspension();
            if (_userService.GetAll().Any(UserCompare => UserCompare.Username == user.Username))
                return new ConflictResult();

            if (_userService.CreateUser(user) == null)
                return Conflict("User scheme is incorrect");

            if (_mailSender != null) {
                _mailSender.Send(user.Email, "Testing AuthoBson", "Testing");
            }

            return CreatedAtRoute("CreateUser", new { id = user.Id.ToString() }, User);
        }

        [Authorize(Roles = "Moderator,Administrator")]
        [HttpPut("{id:length(24)}", Name = "SuspendUser")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public IActionResult Suspend(User initiator, string id, string reason, DateTime duration) {
            if (id == initiator.Id)
                return new BadRequestObjectResult(initiator.Username + " cannot self suspend.");
            if (initiator.ValidateRole())
                return new ForbidResult(initiator.Username + "is not in authority to perform this action.");

            Suspension Suspension = new(reason, duration);

            if (_userService.SuspendUser(Suspension, id) == null)
                return NotFound();

            return NoContent();
        }

        [HttpPut("update/{id:length(24)}", Name = "UpdateUser")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public IActionResult Update(IDictionary<string, object> pairs, string id)
        {
            User user = _userService.UpdateUser(pairs, id);

            if (user == null)
            {
                return NotFound();
            }

            return new ObjectResult(user);
        }

        [HttpDelete("delete/{id:length(24)}", Name = "DeleteUser")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public IActionResult Delete(string id)
        {
            User user = _userService.RemoveUser(id);

            if (user == null)
            {
                return NotFound();
            }

            return new ObjectResult(user);
        }

        [Authorize]
        [HttpGet("register")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public IActionResult RegisterEmailing(string[] args)
        {
            templates = args;
            return new ObjectResult(args);
        }
    }
}
