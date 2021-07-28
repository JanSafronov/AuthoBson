using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Transactions;
using System.Web;
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
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Models;
using Services;


namespace Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase {

        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<BsonUser>> Get() =>
            _userService.GetAll().ToList();

        [HttpGet("{id:length(24)}", Name = "GetUser")]
        public ActionResult<BsonUser> Get(string id) {
            BsonUser user = _userService.GetUser(id);

            if (user == null) {
                return NotFound(user);
            }

            return user;
        }

        [HttpPost]
        public ActionResult<BsonUser> Create(BsonUser user)
        {
            _userService.CreateUser(user);

            return CreatedAtRoute("GetBook", new { id = user.Id.ToString() }, user);
        }

        [Authorize(Policy = "moderate")]
        [HttpPost("{id:length(24)}")]
        public IActionResult Suspend(BsonUser initiator, string id, string reason, DateTime duration) {
            if (!_userService.Moderate(initiator))
                return new UnauthorizedObjectResult(initiator.username + " is not authorized to do this action");
            

            if (_userService.SuspendUser(id, reason, duration) == null)
                return NotFound();

            return NoContent();
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, BsonUser userIn)
        {
            var user = _userService.GetUser(id);

            if (user == null)
            {
                return NotFound();
            }

            _userService.ReplaceUser(id, userIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var user = _userService.GetUser(id);

            if (user == null)
            {
                return NotFound();
            }

            _userService.RemoveUser(user.Id.AsString);

            return NoContent();
        }

        
    }
}