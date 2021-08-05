using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using Microsoft.VisualStudio.TestPlatform;
using AuthoBson.Models;
using AuthoBson.Services;
using MongoDB.Bson;
using AuthoBson.Test.Models.Collections;

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
            bool scheme = suspended.duration.IsValidDateTime && suspended.reason.IsString && suspended.ToBsonDocument().IsBsonDocument;
            Assert.True(scheme, "Suspended scheme is incorrect");
            
        }

        /// <summary>
        /// Tests whether the scheme and functionality (objectivity) of User class is correct
        /// </summary>
        [Fact]
        public void User_IsObjective() {
            bool proof = (user.suspended.duration is null && user.suspended.reason is null) || 
                         (user.suspended.duration is not null && user.suspended.reason is not null);
            
            
        }

        [Theory]
        [ClassData(typeof(TestListValidationGenerator))]
        public void User_IsValidable(Role role, BsonSuspended suspended) {
            User user = new User("", "", "", true, DateTime.Now, role, suspended);

            bool proof = (user.ValidateRole() && user.suspended.duration == DateTime.MinValue) || 
                         (user.ValidateRole() && user.role < Role.Moderator);

            Assert.False(proof, "Users should not be validated");
        }
    }
}

// Открыть окно