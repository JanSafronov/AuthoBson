using System;
using Xunit;
using Microsoft.VisualStudio.TestPlatform;
using AuthoBson.Models;
using AuthoBson.Services;
using MongoDB.Bson;

namespace AuthoBson.Test.Models
{
    public class UserModel_IsSchematic
    {
        public BsonSuspended suspended = new BsonSuspended("Reason", DateTime.Now);

        public User user = new User("username", "password", "email", true, DateTime.Now, Role.Generic);
        
        /// <summary>
        /// Tests whether the scheme of Suspended struct is a schematic BsonDocument
        /// </summary>
        [Fact]
        public void Suspended_IsSchematic()
        {
            BsonSuspended s = new BsonSuspended("Suspension reason", DateTime.Now);

            bool scheme = s.duration.IsValidDateTime && s.reason.IsString && s.ToBsonDocument().IsBsonDocument;
            Assert.True(scheme, "Suspended scheme is incorrect");
            
        }


        [Fact]
        public void User_IsSchematic() {
            
        }
    }
}