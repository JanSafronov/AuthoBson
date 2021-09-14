using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using Microsoft.VisualStudio.TestPlatform;
using AuthoBson.Models;
using AuthoBson.Services;
using MongoDB.Bson;
using AuthoBson.Test.Collections.Generators;

namespace AuthoBson.Test.ModelTests
{
    public class UserTest
    {
        private readonly Suspension Suspension = new("Reason", DateTime.Now);

        private readonly User User = new("Username", "Password", "Email", true, "", DateTime.Now, Role.Generic, new Suspension("string", DateTime.MaxValue));

        [Theory]
        [ClassData(typeof(TestListValidationGenerator))]
        public void User_IsValidable(Role Role, Suspension Suspension) {
            User User = new("", "", "", true, "", DateTime.Now, Role, Suspension);

            bool proof = (User.ValidateRole() && User.Suspension.Duration == DateTime.MaxValue) ||
                         (User.ValidateRole() && User.Role < Role.Moderator);

            Assert.False(proof, "Users should not be validated");
        }
    }
}