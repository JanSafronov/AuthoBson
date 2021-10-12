using System;
using AuthoBson.Shared.Data.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthoBson.Models.Templates {

    public interface IUserTemplate : IModelTemplate { 
        int[] Username { get; set;}

        int[] Password { get; set; }

        int[] Email { get; set; }

        int[] Verified { get; set; }

        int[] Reason { get; set; }

        bool Scheme(int[] Username, int[] Password, int[] Email, int[] Verified, int[] Reason);

        bool IsSchematic(string Username, string Password, string Email, string Verified, string Reason);

        bool IsSchematic(User User);
    }
    
    public class UserTemplate : IUserTemplate, IModelTemplate {
        public int[] Username { get; set; }

        public int[] Password { get; set; }

        public int[] Email { get; set; }

        public int[] Verified { get; set; }

        public int[] Reason { get; set; }

        public bool Scheme(int[] Username, int[] Password, int[] Email, int[] Verified, int[] Reason) { 
            return Username.Length == 2 && Password.Length == 2 && Email.Length == 2 && Verified.Length == 2 && Reason.Length == 2
                   && Username[0] < Username[1] && Password[0] < Password[1] && Email[0] < Email[1] && Verified[0] < Verified[1] && Reason[0] < Reason[1];
        }

        public bool IsSchematic(string Username, string Password, string Email, string Verified, string Reason) => 
        this.Username[0] <= Username.Length && Username.Length < this.Username[1]
        && this.Password[0] <= Password.Length && Password.Length < this.Password[1]
        && this.Email[0] <= Email.Length && Email.Length < this.Email[1]
        && this.Verified[0] <= Verified.Length && Verified.Length < this.Verified[1]
        && this.Reason[0] <= Reason.Length && Reason.Length < this.Reason[1];

        public bool IsSchematic(User User) =>
        this.Username[0] <= User.Username.Length && User.Username.Length < this.Username[1]
        && this.Password[0] <= User.Password.Length && User.Password.Length < this.Password[1]
        && this.Email[0] <= User.Email.Length && User.Email.Length < this.Email[1]
        && this.Verified[0] <= User.Verified.Length && User.Verified.Length < this.Verified[1]
        && this.Reason[0] <= User.Suspension.Reason.Length && User.Suspension.Reason.Length < this.Reason[1];

        /*[BsonConstructor("Username", "Password", "Email", "Verified", "Reason")]
        public UserTemplate(int[] Username, int[] Password, int[] Email, int[] Verified, int[] Reason) {
            if (Scheme(Username, Password, Email, Verified, Reason)) {
                this.Username = Username;
                this.Password = Password;
                this.Email = Email;
                this.Verified = Verified;
                this.Reason = Reason;
            }
        }*/
    }
}