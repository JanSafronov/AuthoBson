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
using AuthoBson.Test.TestData;

namespace AuthoBson.Test.ServiceTests {
    public class UserServiceTest {

        public UserService _userService = UserData.UserService;

        private void Reset() {
            _userService = UserData.UserService;
        }

        [Fact]
        public void UserService_Users_AreAccessible() {
            User user0 = new("Username", "Password", "Email", true, "", DateTime.Now, Role.Generic, new Suspension("string", DateTime.MaxValue));
            User user1 = new User("Username1", "Password1", "Email1", false, "", DateTime.Now, Role.Senior, new Suspension("string", DateTime.MaxValue));

            _userService.CreateUser(user0);
            _userService.CreateUser(user1);

            IEnumerable<IUser> collection = _userService.GetAll();
            Assert.Contains<IUser>(user0, collection);
            Assert.Contains<IUser>(user1, collection);
            
            Reset();
        }

        [Fact]
        public void UserService_User_IsAccessible() {
            User User = new("Username", "Password", "Email", true, "", DateTime.Now, Role.Generic, new Suspension("string", DateTime.MaxValue));
            _userService.CreateUser(User);
            

            Assert.True(_userService.GetUser(User.Id) == User, "User should be accessible");

            Reset();
        }

        [Fact]
        public void UserService_User_IsReplacable() {
            User User = new("Username", "Password", "Email", true, "", DateTime.Now, Role.Generic, new Suspension("string", DateTime.MaxValue));
            User newUser = new("Username1", "Password1", "Email1", false, "", DateTime.Now, Role.Senior, new Suspension("string", DateTime.MaxValue));

            _userService.CreateUser(User);
            _userService.ReplaceUser(User.Id, newUser);

            Assert.DoesNotContain<IUser>(User, _userService.GetAll());
            Assert.Same(newUser, _userService.GetUser(newUser.Id));

            Reset();
        }

        [Fact]
        public void UserService_User_IsSuspendable() {
            User User = new("Username", "Password", "Email", true, "", DateTime.Now, Role.Generic, new Suspension("string", DateTime.MaxValue));
            Suspension Suspension = new("Reason", DateTime.MaxValue);

            _userService.CreateUser(User);
            _userService.SuspendUser(User.Id, Suspension);

            Assert.Same(_userService.GetUser(User.Id).Suspension, Suspension);

            Reset();
        }

        [Fact]
        public void UserService_User_IsRemovable() {
            User User = new("Username", "Password", "Email", true, "", DateTime.Now, Role.Generic, new Suspension("string", DateTime.MaxValue));
            _userService.CreateUser(User);

            _userService.RemoveUser(User.Id);

            Assert.Empty(_userService.GetAll());

            Reset();
        }

        [Fact]
        public void UserService_Field_IsChangable() {
            IUser User = new User("Username", "Password", "Email", true, "", DateTime.Now, Role.Generic, new Suspension("string", DateTime.MaxValue));
            _userService.CreateUser(User);

            User = _userService.ChangeField<BsonInt32>(User.Id, "notification", e => e == true ? 1 : 0);

            Reset();
        }
    }
}