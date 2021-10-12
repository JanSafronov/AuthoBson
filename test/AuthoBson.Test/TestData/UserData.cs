using System;
using System.Collections;
using AuthoBson.Models;
using AuthoBson.Models.Templates;
using AuthoBson.Services;
using AuthoBson.Shared.Data.Models;

namespace AuthoBson.Test.TestData {
    public static class UserData {
        public static User User = new User(
            Username: "Username",
            Password: "Password",
            Email: "Username@address.domain",
            Notification: true,
            Verified: "",
            Joined: DateTime.MinValue,
            Role: Role.Senior,
            Suspension: new Suspension(
                Reason: "Suspension reason",
                Duration: DateTime.MaxValue
            )
        );

        public static UserTemplate UserTemplate = new UserTemplate() {
            Username = new int[2] { 10, 50 },
            Password = new int[2] { 10, 100 },
            Email = new int[2] { 20, 320 },
            Verified = new int[2] { 10, 200},
            Reason = new int[2] {10, 200}
        };

        public static UserService UserService = new UserService(new UserstoreDatabase() {
            UsersCollectionName = "Users",
            DatabaseName = "Profiles"
        }, UserTemplate);
    }
}