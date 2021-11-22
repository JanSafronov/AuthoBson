using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Threading.Tasks.Sources;
using AuthoBson.Services;
using AuthoBson.Models;
using Xunit;

namespace AuthoBson.IntegrationTest
{
    public class UserService_IntegrationTest
    {
        public Task<UserService> service = AsyncTests.CreateAsyncTestService();

        [Fact]
        public void UserService_IsConnectable()
        {
            Assert.True(service.Result is not null, "User service should be testable");
        }
    }
}
