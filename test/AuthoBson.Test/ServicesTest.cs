using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Bson;
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

        private void reset() {
            _userService = new UserService(new UserstoreDatabase() {
                UsersCollectionName = "Users",
                DatabaseName = "Profiles"
            });
        }

        [Fact]
        public void UserService_Users_AreAccessible() {
            User user0 = new User("username", "password", "email", true, DateTime.Now, Role.Generic);
            User user1 = new User("username1", "password1", "email1", false, DateTime.Now, Role.Senior);

            _userService.CreateUser(user0);
            _userService.CreateUser(user1);

            IEnumerable<User> collection = _userService.GetAll();
            Assert.Contains<User>(user0, collection);
            Assert.Contains<User>(user1, collection);

            reset();
        }
        
        [Fact]
        public void UserService_User_IsInsertable() {
            User user = new User("username", "password", "email", true, DateTime.Now, Role.Generic);
            _userService.CreateUser(user);
            
            Assert.Same(user, _userService.GetUser(user.Id));

            reset();
        }

        [Fact]
        public void UserService_User_IsAccessible() {
            User user = new User("username", "password", "email", true, DateTime.Now, Role.Generic);
            _userService.CreateUser(user);

            Assert.True(_userService.GetUser(user.Id) is User, "User should be accessible");

            reset();
        }

        [Fact]
        public void UserService_User_IsReplacable() {
            User user = new User("username", "password", "email", true, DateTime.Now, Role.Generic);
            User newuser = new User("username1", "password1", "email1", false, DateTime.Now, Role.Senior);

            _userService.CreateUser(user);
            _userService.ReplaceUser(user.Id, newuser);

            Assert.DoesNotContain<User>(user, _userService.GetAll());
            Assert.Same(newuser, _userService.GetUser(newuser.Id));

            reset();
        }

        [Fact]
        public void UserService_User_IsSuspendable() {
            User user = new User("username", "password", "email", true, DateTime.Now, Role.Generic);
            Suspension suspension = new Suspension("reason", DateTime.MaxValue);

            _userService.CreateUser(user);
            _userService.SuspendUser(user.Id, suspension);

            Assert.Same(_userService.GetUser(user.Id).suspension, suspension);

            reset();
        }

        [Fact]
        public void UserService_User_IsRemovable() {
            User user = new User("username", "password", "email", true, DateTime.Now, Role.Generic);
            _userService.CreateUser(user);

            _userService.RemoveUser(user.Id);

            Assert.Empty(_userService.GetAll());

            reset();
        }

        [Fact]
        public void UserService_Field_IsChangable() {
            User user = new User("username", "password", "email", true, DateTime.Now, Role.Generic);
            _userService.CreateUser(user);

            user = _userService.ChangeField<BsonInt32>(user.Id, "notification", e => e == true ? 1 : 0);

            
        }
    }
}