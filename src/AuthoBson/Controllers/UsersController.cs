using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using System.Transactions;
using System.Security.Policy;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Logging;
using Microsoft.Extensions.DependencyInjection;
using AuthoBson.Models;
using AuthoBson.Services;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using AuthoBson.Shared.Services.Results;
using AuthoBson.Network;
using MongoDB.Driver;

namespace AuthoBson.Controllers
{

    [ApiController]
    [Route("api/user", Name = "user")]
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

        [HttpGet("{id:length(24)}/async", Name = "GetUserAsync")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public async Task<ActionResult<User>> GetAsync(string id)
        {
            User user = await _userService.GetUserAsync(id);

            if (user == null)
            {
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

            if (_mailSender != null)
                _mailSender.Send(user.Email, templates[0], templates[1]);

            return CreatedAtRoute("CreateUser", new { id = user.Id.ToString() }, User);
        }

        [HttpPost("async", Name = "CreateUserAsync")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public async Task<IActionResult> CreateAsync(User user)
        {
            user.Suspension = new Suspension();
            if (_userService.GetAll().Any(UserCompare => UserCompare.Username == user.Username))
                return new ConflictResult();

            if (_userService.CreateUserAsync(user) == null)
                return Conflict("User scheme is incorrect");

            if (_mailSender != null)
                await _mailSender.SendAsync(user.Email, templates[0], templates[1]);

            return CreatedAtRoute("CreateUser", new { id = user.Id.ToString() }, User);
        }

        [HttpPut("{initiatorId:length(24)}/{targetId:length(24)}", Name = "SuspendUser")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public IActionResult Suspend(string initiatorId, string targetId, string reason, DateTime duration) {
            User initiator = _userService.GetUser(initiatorId);
            if (initiator == null)
                return NotFound(initiator);
            if (initiatorId == targetId)
                return new BadRequestObjectResult(initiator.Username + " cannot self suspend.");
            if (initiator.ValidateRole())
                return new ForbidResult(initiator.Username + "is not in authority to perform this action.");

            Suspension Suspension = new(reason, duration);

            if (_userService.SuspendUser(Suspension, targetId) == null)
                return NotFound();

            return NoContent();
        }

        [HttpPut("{initiatorId:length(24)}/{targetId:length(24)}/async", Name = "SuspendUserAsync")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public async Task<IActionResult> SuspendAsync(string initiatorId, string targetId, string reason, DateTime duration)
        {
            User initiator = await _userService.GetUserAsync(initiatorId);
            if (initiator == null)
                return NotFound(initiator);
            if (initiatorId == targetId)
                return new BadRequestObjectResult(initiator.Username + " cannot self suspend.");
            if (initiator.ValidateRole())
                return new ForbidResult(initiator.Username + "is not in authority to perform this action.");

            Suspension Suspension = new(reason, duration);

            if (_userService.SuspendUserAsync(Suspension, targetId) == null)
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

        [HttpPut("update/single/{id:length(24)}", Name = "UpdateUserSingle")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public IActionResult UpdateSingle(string key, object value, string id)
        {
            User user = _userService.UpdateUser(KeyValuePair.Create(key, value), id);

            if (user == null)
            {
                return NotFound();
            }

            return new ObjectResult(user);
        }

        [HttpPut("update/{id:length(24)}/async", Name = "UpdateUserAsync")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public async Task<IActionResult> UpdateAsync(IDictionary<string, object> pairs, string id)
        {
            User user = await _userService.UpdateUserAsync(pairs, id);

            if (user == null)
            {
                return NotFound();
            }

            return new ObjectResult(user);
        }

        [HttpPut("update/single/{id:length(24)}/async", Name = "UpdateUserSingleAsync")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public async Task<IActionResult> UpdateSingleAsync(KeyValuePair<string, object> pair, string id)
        {
            User user = await _userService.UpdateUserAsync(pair, id);

            if (user == null)
            {
                return NotFound();
            }

            return new ObjectResult(user);
        }

        [HttpPut("replace/{id:length(24)}", Name = "ReplaceUser")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public IActionResult Replace(User newUser, string id)
        {
            User user = _userService.ReplaceUser(newUser, id);

            if (user == null)
            {
                return NotFound();
            }

            return new ObjectResult(user);
        }

        [HttpPut("replace/{id:length(24)}/async", Name = "ReplaceUserAsync")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public async Task<IActionResult> ReplaceAsync(User newUser, string id)
        {
            User user = await _userService.ReplaceUserAsync(newUser, id);

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
        public async Task<IActionResult> Delete(string id)
        {
            User user = await _userService.RemoveUserAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return new ObjectResult(user);
        }

        [HttpDelete("delete/{id:length(24)}/async", Name = "DeleteUserAsync")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            User user = await _userService.RemoveUserAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return new ObjectResult(user);
        }

        [HttpPost("register")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public IActionResult RegisterEmailing(params string[] args)
        {
            templates = args;
            return new ObjectResult(args);
        }
    }
}
