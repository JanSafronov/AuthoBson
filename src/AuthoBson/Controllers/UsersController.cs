using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Security.Policy;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using AuthoBson.Models;
using AuthoBson.Services;
using AuthoBson.Protocols;
using AuthoBson.Protocols.Settings;
using MongoDB.Bson;


namespace AuthoBson.Controllers {

    [ApiController]
    [Route("api/[controller]", Name = "User")]
    public class UserController : ControllerBase {

        private readonly UserService _userService;

        private readonly MailSender _mailSender;

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
        public ActionResult<List<User>> Get() =>
            _userService.GetAll();
        
        [HttpGet("{id:length(24)}", Name = "GetUser")]
        public ActionResult<User> Get(string Id) {
            IUser User = _userService.GetUser(Id);

            if (User == null) {
                return NotFound(User);
            }
            
            return new ObjectResult(User);
        }

        [HttpPost]
        public IActionResult Create(User User)
        {
            User.Suspension = new Suspension();
            if (_userService.GetAll().Any(UserCompare => UserCompare.Username == User.Username))
                return new ConflictResult();

            if (_userService.CreateUser(User) == null)
                return Conflict("User scheme is incorrect");

            if (_mailSender != null) {
                _mailSender.Send(User.Email, "Testing AuthoBson", "Testing");
            }

            return CreatedAtRoute("GetUser", new { id = User.Id.ToString() }, User);
        }

        [Authorize(Policy = "moderate")]
        [HttpPost("{id:length(24)}")]
        public IActionResult Suspend(User Initiator, string Id, string Reason, DateTime Duration) {
            if (Id == Initiator.Id)
                return new BadRequestObjectResult(Initiator.Username + " cannot self suspend.");
            if (Initiator.ValidateRole())
                return new ForbidResult(Initiator.Username + "is not in autority to perform this action.");

            Suspension Suspension = new(Reason, Duration);

            if (_userService.SuspendUser(Id, Suspension) == null)
                return NotFound();

            return NoContent();
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string Id, User UserIn)
        {
            var user = _userService.GetUser(Id);

            if (user == null)
            {
                return NotFound();
            }

            _userService.ReplaceUser(Id, UserIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string Id)
        {
            IUser User = _userService.GetUser(Id);

            if (User == null)
            {
                return NotFound();
            }

            _userService.RemoveUser(User.Id);

            return NoContent();
        }
    }
}