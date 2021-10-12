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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using AuthoBson.Models;
using AuthoBson.Services;
using AuthoBson.Protocols;
using AuthoBson.Protocols.Settings;


namespace AuthoBson.Controllers {

    [ApiController]
    [Route("api/[controller]", Name = "User")]
    public class UserController : ControllerBase {

        private readonly UserService _userService;

        private readonly IMailSender _mailSender;

        public UserController(UserService userService) //, IDomainSettings mailSettings)
        {
            _userService = userService;
            //_mailSender = new SMTPMail(mailSettings)
        }

        /*public UserController(UserService userService)
        {
            _userService = userService;
            _mailSender = null;
        }*/

        [HttpGet(Name = "GetUsers")]
        public ActionResult<IEnumerable<User>> Get() =>
            _userService.GetAll().ToList();

        [HttpGet("{id:length(24)}", Name = "GetUser")]
        public ActionResult<User> Get(string id) {
            User User = _userService.GetUser(id);

            if (User == null) {
                return NotFound(User);
            }
            
            return User;
        }

        [HttpPost]
        public ActionResult<User> Create(User User)
        {
            if (_userService.GetAll().Any(User => User.Username == User.Username))
                return new ConflictResult();

            if (_userService.CreateUser(User) == null)
                return Conflict("User scheme is incorrect");

            /*if (_mailSender != null) {
                _mailSender.
            }*/

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
            User User = _userService.GetUser(Id);

            if (User == null)
            {
                return NotFound();
            }

            _userService.RemoveUser(User.Id);

            return NoContent();
        }
    }
}