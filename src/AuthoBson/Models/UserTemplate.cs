using System;
using AuthoBson.Shared.Data.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthoBson.Models
{

    public class UserTemplate : IModelTemplate<User>
    {
        public int[] Username { get; set; }

        public int[] Password { get; set; }

        public int[] Email { get; set; }

        public int[] Verified { get; set; }

        public int[] Reason { get; set; }

        public static bool Scheme(int[] Username, int[] Password, int[] Email, int[] Verified, int[] Reason) =>
            Scheme(Username, Password, Email, Verified, Reason);

        public static bool Scheme(UserTemplate template) =>
            Scheme(template.Username, template.Password, template.Email, template.Verified, template.Reason);

        public bool IsSchematic(string Username, string Password, string Email, string Verified, string Reason) =>
            this.Username[0] <= Username.Length && Username.Length < this.Username[1]
            && this.Password[0] <= Password.Length && Password.Length < this.Password[1]
            && this.Email[0] <= Email.Length && Email.Length < this.Email[1]
            && this.Verified[0] <= Verified.Length && Verified.Length < this.Verified[1]
            && this.Reason[0] <= Reason.Length && Reason.Length < this.Reason[1];

        public override bool IsSchematic(User User) =>
            IsSchematic(User.Username, User.Password, User.Email, User.Verified, User.Suspension.Reason);
    }
}
