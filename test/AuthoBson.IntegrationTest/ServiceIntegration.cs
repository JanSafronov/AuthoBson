using System;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Driver.Core.Authentication;
using MongoDB.Driver.Core.Bindings;
using MongoDB.Driver.Core.Clusters;
using MongoDB.Driver.Core.Clusters.ServerSelectors;
using MongoDB.Driver.Core.Compression;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Connections;
using MongoDB.Driver.Core.Servers;
using MongoDB.Driver.Linq;
using MongoDB.Bson.Serialization;
using AuthoBson.Services;
using AuthoBson.Models;
using AuthoBson.IntegrationTest.Services;
using Xunit;

namespace AuthoBson.IntegrationTest
{
    public class UserService_IntegrationTest
    {
        /// <summary>
        /// Initialize testable user service service
        /// </summary>
        /// <returns>Testable user service</returns>
        public UserService CreateTestService() {
            
            MongoClient client = new MongoClient();

            IUserstoreDatabaseSettings settings = new UserstoreDatabaseSettings() {
                UsersCollectionName = "Users",
                ConnectionString = "mongodb://testhost",
                DatabaseName = "Profiles"
            };

            return new UserService(settings);
        }

        [Fact]
        public void UserService_IsConnectable()
        {
            UserService service = CreateTestService();

            Assert.True(service is UserService, "User service should be testable");
        }
    }
}
