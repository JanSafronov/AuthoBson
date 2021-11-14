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
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace AuthoBson.Controllers
{
    [ApiController]
    [Route("api/account")]
    [Authorize]
    [RequireHttps]
    public class AccountController : ControllerBase
    {

        private readonly UserService _userService;

        [ActivatorUtilitiesConstructor]
        public AccountController(UserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("{username}", Name = "Login")]
        public ActionResult<User> Login(string Username, string Password) =>
            _userService.LoginUser(Username, Password);

        [HttpPut(Name = "Logout")]
        public void Logout(string Username) =>
            _userService.UpdateUser(Username, new KeyValuePair<string, object>("Active", false));
    }
}
