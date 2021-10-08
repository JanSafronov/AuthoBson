using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Xunit;
using AuthoBson.Services;
using AuthoBson.Models;
using AuthoBson.Controllers;
using AuthoBson.Test.TestData;
using AuthoBson.Shared.Data.Models;

namespace AuthoBson.IntegrationTest.Async {
    public class AsyncTests {

        /// <summary>
        /// Initialize testable user service
        /// </summary>
        /// <returns>Testable user service</returns>
        public static async Task<UserService> CreateAsyncTestService() {
            
            MongoClient client = new();

            IUserstoreDatabaseSettings settings = new UserstoreDatabaseSettings() {
                DatabaseName = "Profiles",
                ConnectionString = "mongodb://testhost",
                UsersCollectionName = "Users"
            };

            Task<UserService> service = Task.Run<UserService>(() => {
                return new UserService(settings, UserData.UserTemplate);
            });

            return await service;
        }

        /// <summary>
        /// Initialize testable user controller
        /// </summary>
        /// <returns>Testable user controller</returns>
        public static async Task<UserController> CreateAsyncTestController() {
            
            UserService service = await CreateAsyncTestService();

            return new UserController(service);
        }
    }
}