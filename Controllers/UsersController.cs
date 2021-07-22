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
    [Route("[controller]")]
    public class UserController {

        [HttpGet("{username}")]
        public ActionResult<User> GetUser(string identificator) {
            UserService.GetUser(username);
        }

        [HttpGet("{username}")]
        public ActionResult<User> UserType(string identificator) {
            
        }

        [HttpPost("{username}")]
        public void SuspendUser(string username, string reason) {
            
        }

        
    }
}