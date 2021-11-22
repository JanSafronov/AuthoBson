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
using Microsoft.Extensions.DependencyInjection;
using AuthoBson.Models;
using AuthoBson.Services;
using AuthoBson.Network;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Mvc.Abstractions;
using AuthoBson.Shared.Services.Results;

namespace AuthoBson.Controllers
{
    [ApiController]
    [Route("api/account")]
    [Authorize(AuthenticationSchemes = "{username}")]
    [RequireHttps]
    public class AccountController : ControllerBase
    {

        private readonly UserService _userService;

        public string[] templates { get; set; }

        [ActivatorUtilitiesConstructor]
        public AccountController(UserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("{username}", Name = "Login")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public ActionResult<User> Login(string username, string password) =>
            _userService.LoginUser(username, password);

        [AllowAnonymous]
        [HttpPost("{username}/async", Name = "LoginAsync")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public async Task<ActionResult<User>> LoginAsync(string username, string password) =>
            await _userService.LoginUserAsync(username, password);

        [HttpPut(Name = "Logout")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public void Logout(string id) =>
            _userService.UpdateUser(new KeyValuePair<string, object>("Active", false), id);

        [HttpPut("async", Name = "LogoutAsync")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public async Task<User> LogoutAsync(string id) =>
            await _userService.UpdateUserAsync(new KeyValuePair<string, object>("Active", false), id);

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
