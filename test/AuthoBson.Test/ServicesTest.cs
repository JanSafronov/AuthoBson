using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using Xunit.Sdk;
using Xunit.Extensions;
using Xunit.Abstractions;
using AuthoBson.Services;
using AuthoBson.Services.Security;
using AuthoBson.Models;

namespace AuthoBson.Test.ServiceTests {
    public class UserServiceTest {

        public UserService _userService = new UserService(new UserstoreDatabase() {
            UsersCollectionName = "Users",
            DatabaseName = "Profiles"
        });

        [Fact]
        public void UserService_Users_AreAccessible() {
           User user0 = new User("username", "password", "email", true, DateTime.Now, Role.Generic);
           User user1 = new User("username1", "password1", "email1", false, DateTime.Now, Role.Senior);

           _userService.CreateUser(user0);
           _userService.CreateUser(user1);

           IEnumerable<User> collection = _userService.GetAll();
           Assert.Contains<User>(user0, collection);
           Assert.Contains<User>(user1, collection);
        }
        
        public void UserService_User_IsInsertable(User user) {
            _userService.CreateUser(user);
            
            Assert.Same(user, _userService.GetUser(user.Id));
        }

        [Fact]
        public void UserService_User_IsAccessible() {
            User user = new User("username", "password", "email", true, DateTime.Now, Role.Generic);
            _userService.CreateUser(user);

            Assert.True(_userService.GetUser(user.Id) is User, "User should be accessible");
        }

        [Fact]
        public void UserService_User_IsReplacable() {

        }

        [Theory]
        public void UserService_User_IsSuspendable() {

        }

        [Fact]
        public void UserService_User_IsRemovable() {

        }

        [Fact]
        public void UserService_Field_IsChangable() {

        }
    }
}