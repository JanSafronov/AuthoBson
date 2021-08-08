using System;
using System.Collections;
using AuthoBson.Services;
using AuthoBson.Services.Security;
using AuthoBson.Models;
using Xunit;
using Xunit.Sdk;
using Xunit.Extensions;
using Xunit.Abstractions;

namespace AuthoBson.Test.Services {
    public class UserServiceTest {

        [Fact]
        public void UserService_User_IsInsertable() {
            User user = new User("username", "password", "email", true, DateTime.Now, Role.Generic);
            _userService.CreateUser(user);
            
            UserService_User_IsAccessible();
            Assert.Same(user, _userService.GetUser(user.Id.AsString));
        }

        [Fact]
        private void UserService_User_IsAccessible() {
            
        }
    }
}