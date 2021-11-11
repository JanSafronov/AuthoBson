using System;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using AuthoBson.Controllers;
using AuthoBson.Shared.Results;

using Swashbuckle.AspNetCore.Annotations;

namespace AuthoBson.Controllers
{
    public static class UsersControllerExtension
    {
        [HttpGet(Name = "GetUsers")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Okay", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Conflict", typeof(ErrorResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request", typeof(ErrorResult))]
        public static IActionResult RegisterEmailing(this UserController controller, string[] args)
        {
            controller.templates = args;
            return new ObjectResult(args);
        }
    }
}
