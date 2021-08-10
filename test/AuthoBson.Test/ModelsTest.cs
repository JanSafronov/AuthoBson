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
        private Suspension suspension = new Suspension("Reason", DateTime.Now);

        private User user = new User("username", "password", "email", true, DateTime.Now, Role.Generic);

        [Theory]
        [ClassData(typeof(TestListValidationGenerator))]
        public void User_IsValidable(Role role, Suspension suspension) {
            User user = new User("", "", "", true, DateTime.Now, role, suspension);

            bool proof = (user.ValidateRole() && user.suspension.duration == DateTime.MaxValue) ||
                         (user.ValidateRole() && user.role < Role.Moderator);

            Assert.False(proof, "Users should not be validated");
        }
    }
}