using System;
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

        public UserController(UserService userService) :
            this(userService, null)
        { }

        [HttpGet(Name = "GetUsers")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public ActionResult<List<User>> Get() =>
            _userService.GetAll();
        
        [HttpGet("{Id:length(24)}", Name = "GetUser")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public ActionResult<User> Get(string Id) {
            IUser User = _userService.GetUser(Id);

            if (User == null) {
                return NotFound(User);
            }
            
            return new ObjectResult(User);
        }

        [HttpPost(Name = "CreateUser")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public IActionResult Create(User User)
        {
            User.Suspension = new Suspension();
            if (_userService.GetAll().Any(UserCompare => UserCompare.Username == User.Username))
                return new ConflictResult();

            if (_userService.CreateUser(User) == null)
                return Conflict("User scheme is incorrect");

            if (_mailSender != null) {
                _mailSender.Send(User.Email, templates[0], templates[1]);
            }

            return CreatedAtRoute("CreateUser", new { id = User.Id.ToString() }, User);
        }

        [Authorize(Policy = "moderate")]
        [HttpPut("{Id:length(24)}", Name = "SuspendUser")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public IActionResult Suspend(User Initiator, string Id, string Reason, DateTime Duration) {
            if (Id == Initiator.Id)
                return new BadRequestObjectResult(Initiator.Username + " cannot self suspend.");
            if (Initiator.ValidateRole())
                return new ForbidResult(Initiator.Username + "is not in authority to perform this action.");

            Suspension Suspension = new(Reason, Duration);

            if (_userService.SuspendUser(Suspension, Id) == null)
                return NotFound();

            return NoContent();
        }

        [HttpPut("update/{Id:length(24)}", Name = "UpdateUser")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public IActionResult Update(User UserIn, string Id)
        {
            var User = _userService.ReplaceUser(UserIn, Id);

            if (User == null)
            {
                return NotFound();
            }

            return new ObjectResult(User);
        }

        [HttpDelete("delete/{Id:length(24)}", Name = "DeleteUser")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public IActionResult Delete(string Id)
        {
            User User = _userService.RemoveUser(Id);

            if (User == null)
            {
                return NotFound();
            }

            return new ObjectResult(User);
        }
    }
}
