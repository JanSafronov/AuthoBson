using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Threading.Tasks.Sources;
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

        public Task<UserController> controller = AsyncTests.CreateTestController();

        [Fact]
        public void Controller_IsIntegrable() {
            Assert.True(controller.Result is UserController, "User controller should be testable");
        }

        [Fact]
        public async Task Controller_IsAccessible() {
            UserController asyncController = await controller;

            User user = new User("username", "password", "email", true, "", DateTime.Now, Role.Generic);

            asyncController.Create(user);
        }
    }
}