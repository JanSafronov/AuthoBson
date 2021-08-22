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

            User user = new User("username", "password", "email", true, "", DateTime.Now, Role.Generic);

            asyncController.Create(user);

            User actual = asyncController.Get(user.Id).Value;

            Assert.Same(user, actual);
        }

        [Fact]
        public async Task Controller_Users_AreAccessible() {
            UserController asyncController = await controller;

            User user0 = new User("username", "password", "email", true, "", DateTime.Now, Role.Generic);
            User user1 = new User("username1", "password1", "email1", false, "", DateTime.Now, Role.Senior);

            asyncController.Create(user0);
            asyncController.Create(user1);

            IEnumerable<User> collection = asyncController.Get().Value;
            Assert.Contains<User>(user0, collection);
            Assert.Contains<User>(user1, collection);
        }

        [Theory]
        [ClassData(typeof(AuthoBson.Test.Collections.Generators.TestListValidationGenerator))]
        public async Task Controller_User_IsSuspendable(Role role, Suspension suspension) {
            UserController asyncController = await controller;

            User initiator = new User("username", "password", "email", true, "", DateTime.Now, role, suspension);
            User user = new User("username1", "password1", "email1", false, "", DateTime.Now, role, suspension);

            bool proof = (initiator.ValidateRole() && initiator.suspension.duration == DateTime.MaxValue) ||
                         (initiator.ValidateRole() && initiator.role < Role.Moderator);

            asyncController.Suspend(initiator, user.Id, "reason", DateTime.MaxValue);

            Assert.True(proof, "Initiator should be able to suspend");
        }
    }
}