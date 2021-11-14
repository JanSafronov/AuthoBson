using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using AuthoBson.Models.Templates;
using AuthoBson.Shared;
using AuthoBson.Shared.Data.Models;

namespace AuthoBson.Models {

    public enum Role { Generic, Senior, Moderator, Administrator }

    public interface IUser : IModelBase {

        string Username { get; set; }

        string Password { get; set; }

        string Email { get; set; }

        Boolean Notification { get; set; }

        DateTime Joined { get; }

        Role Role { get; set; }

        string Verified { get; set; }

        bool Active { get; set; }

        Suspension Suspension { get; set; }

        string Salt { get; }

        bool ValidateRole();
    }

    /// <summary>
    /// Abstract implementation of user interface
    /// </summary>
    /// <remarks>
    /// Not recommended for documentless use due to bson documents and morphism incapabilities
    /// </remarks>
    [BsonDiscriminator("UserBase")]
    [BsonKnownTypes(typeof(User))]
    [Obsolete("UserBase is deprecated, please use other models instead")]
    public abstract class UserBase
    {

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public Boolean Notification { get; set; }

        public DateTime Joined { get; set; }

        public Role Role { get; set; }

        public string Verified { get; set; }

        public bool Active { get; set; }

        public Suspension Suspension { get; set; }

        public string Salt { get; set; }

        public UserBase(string Username, string Password, string Email, bool Notification, string Verified, DateTime Joined, Role Role) :
            base()
        {
            this.Username = Username;
            this.Password = Password;
            this.Email = Email;
            this.Notification = Notification;
            this.Joined = Joined;
            this.Role = Role;
            this.Verified = Verified;
            this.Active = true;
        }

        public UserBase(string Username, string Password, string Email, bool Notification, string Verified, DateTime Joined, Role Role, Suspension Suspension) :
            this(Username, Password, Email, Notification, Verified, Joined, Role)
        {
            this.Suspension = Suspension;
        }

        public abstract bool ValidateRole();
    }

    [BsonDiscriminator("User")]
    [MessageIn][MessageOut]
    public class User : ModelBase, IUser {

        [Unique("Username")]
        [BsonElement("Username")]
        [JsonProperty("Username")]
        [BsonRepresentation(BsonType.String)]
        public string Username { get; set; }

        [BsonElement("Password")]
        [JsonProperty("Password")]
        [BsonRepresentation(BsonType.String)]
        public string Password { get; set; }

        [Unique("Email")]
        [BsonElement("Email")]
        [JsonProperty("Email")]
        [BsonRepresentation(BsonType.String)]
        public string Email { get; set; }

        [BsonElement("Notification")]
        [JsonProperty("Notification")]
        [BsonRepresentation(BsonType.Boolean)]
        public Boolean Notification { get; set; }

        [BsonElement("Joined")]
        [JsonProperty("Joined")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime Joined { get; }

        [BsonElement("Role")]
        [JsonProperty("Role")]
        [BsonRepresentation(BsonType.Int32)]
        public Role Role { get; set; }

        [BsonElement("Verified")]
        [JsonProperty("Verified")]
        [BsonRepresentation(BsonType.String)]
        public string Verified { get; set; }

        [BsonElement("Active")]
        [JsonProperty("Active")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool Active { get; set; }

        [BsonElement("Suspension")]
        [JsonProperty("Suspension")]
        public Suspension Suspension { get; set; }

        [BsonElement("Salt")]
        [JsonProperty("Salt")]
        [BsonRepresentation(BsonType.String)]
        public string Salt { get; internal set; }

        [BsonConstructor("Username", "Password", "Email", "Notification", "Joined", "Role", "Verified", "Suspension")]
        [JsonConstructor]
        public User(string Username, string Password, string Email, bool Notification, string Verified, DateTime Joined, Role Role, Suspension Suspension) :
            base()
        {
            this.Username = Username;
            this.Password = Password;
            this.Email = Email;
            this.Notification = Notification;
            this.Joined = Joined;
            this.Role = Role;
            this.Verified = Verified;
            this.Active = true;
            this.Suspension = Suspension;
        }

        public User(string Id, string Username, string Password, string Email, bool Notification, string Verified, DateTime Joined, Role Role, Suspension Suspension) :
            base(Id)
        {
            this.Username = Username;
            this.Password = Password;
            this.Email = Email;
            this.Notification = Notification;
            this.Joined = Joined;
            this.Role = Role;
            this.Verified = Verified;
            this.Active = true;
            this.Suspension = Suspension;
        }

        public bool ValidateRole() {
            bool proof = Role >= Role.Moderator;

            proof = proof && Suspension.Duration < DateTime.Now;
            
            return proof;
        }
    }
}
