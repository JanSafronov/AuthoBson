using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Threading.Tasks.Sources;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions;
using Xunit.Sdk;
using AuthoBson.Models;
using AuthoBson.Controllers;
using AuthoBson.Services;
using AuthoBson.IntegrationTest.Async;
using MongoDB.Bson;

namespace AuthoBson.IntegrationTest.Controllers {
    public class UserController_IntegrationTest {

        public Task<UserController> controller = AsyncTests.CreateAsyncTestController();

        [Fact]
        public void Controller_IsIntegrable() {
            Assert.True(controller.Result is UserController, "User controller should be testable");
        }

        [Fact]
        public async Task Controller_User_IsAccessible() {
            UserController asyncController = await controller;

            User User = new("Username", "Password", "Email", true, "", DateTime.Now, Role.Generic, new Suspension("string", DateTime.MaxValue));

            asyncController.Create(User);

            User actual = asyncController.Get(User.Id).Value;

            Assert.Same(User, actual);
        }

        [Fact]
        public async Task Controller_Users_AreAccessible() {
            UserController asyncController = await controller;

            User user0 = new("Username", "Password", "Email", true, "", DateTime.Now, Role.Generic, new Suspension("string", DateTime.MaxValue));
            User user1 = new("Username1", "Password1", "Email1", false, "", DateTime.Now, Role.Senior, new Suspension("string", DateTime.MaxValue));

            asyncController.Create(user0);
            asyncController.Create(user1);

            IEnumerable<IUser> collection = asyncController.Get().Value;
            Assert.Contains<IUser>(user0, collection);
            Assert.Contains<IUser>(user1, collection);
        }

        [Theory]
        [ClassData(typeof(AuthoBson.Test.Utilities.Generators.TestListValidationGenerator))]
        public async Task Controller_User_IsSuspendable(Role Role, Suspension Suspension) {
            UserController asyncController = await controller;

            User initiator = new("Username", "Password", "Email", true, "", DateTime.Now, Role, Suspension);
            User user = new("Username1", "Password1", "Email1", false, "", DateTime.Now, Role, Suspension);

            bool proof = (initiator.ValidateRole() && initiator.Suspension.Duration == DateTime.MaxValue) ||
                         (initiator.ValidateRole() && initiator.Role < Role.Moderator);

            asyncController.Suspend(initiator, user.Id, "reason", DateTime.MaxValue);

            Assert.True(proof, "Initiator should be able to suspend");
        }

        [Fact]
        public async Task Controller_User_IsReplacable() {
            UserController asyncController = await controller;

            User User = new("Username", "Password", "Email", true, "", DateTime.Now, Role.Generic, new Suspension("string", DateTime.MaxValue));
            User newUser = new("Username1", "Password1", "Email1", false, "", DateTime.Now, Role.Senior, new Suspension("string", DateTime.MaxValue));

            asyncController.Create(User);
            asyncController.Update(newUser, User.Id);

            Assert.Same(asyncController.Get(newUser.Id).Value, newUser);
        }

        [Fact]
        public async Task Controller_User_IsRemovable() {
            UserController asyncController = await controller;

            User User = new("Username", "Password", "Email", true, "", DateTime.Now, Role.Generic, new Suspension("string", DateTime.MaxValue));

            asyncController.Create(User);
            asyncController.Delete(User.Id);

            Assert.DoesNotContain<IUser>(User, asyncController.Get().Value);
        }
    }
}
