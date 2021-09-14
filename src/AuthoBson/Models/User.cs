using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using Newtonsoft.Json;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;

namespace AuthoBson.Models {

    public enum Role { Generic, Senior, Moderator, Administrator }

    [BsonDiscriminator("Suspension")]
    public class Suspension {

        [BsonElement("Reason")]
        [JsonProperty("Reason")]
        [BsonRepresentation(BsonType.String)]
        public String Reason { get; set; }

        [BsonElement("Duration")]
        [JsonProperty("Duration")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime Duration { get; set; }

        [BsonConstructor("Reason", "Duration")]
        public Suspension(string Reason, DateTime Duration) {
            this.Reason = Reason;
            this.Duration = Duration;
        }
    }

    public interface IUser {

        string Id { get; }

        string Username { get; set; }

        string Password { get; set; }

        string Email { get; set; }

        Boolean Notification { get; set; }

        DateTime Joined { get; }

        Role Role { get; set; }

        string Verified { get; set; }

        bool Active { get; set; }

        Suspension Suspension { get; set; }

        bool ValidateRole();
    }

    public interface IBsonUserDocument {

        BsonDocument User { get; set; }
        
        /// <summary>
        /// Functor mapping between fields and preserving the initial type
        /// </summary>
        /// <param name="key">Identity of the field</param>
        /// <param name="functor">Pattern of mapping</param>
        /// <returns>User object with a field mapped by the functor</returns>
        UserDocument Functor<B>(string key, Func<BsonValue, B> functor) where B : BsonValue;
    }

    /// <summary>
    /// Abstract implementation of generic user interface
    /// </summary>
    /// <remarks>
    /// Not recommended for documentless use due to bson documents and morphism incapabilities
    /// </remarks>
    [BsonDiscriminator("UserBase")]
    [BsonKnownTypes(typeof(User))]
    public abstract class UserBase : IUser {

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; }

        [BsonElement("Username")]
        [JsonProperty("Username")]
        [BsonRepresentation(BsonType.String)]
        public string Username { get; set; }

        [BsonElement("Password")]
        [JsonProperty("Password")]
        [BsonRepresentation(BsonType.String)]
        public string Password { get; set; }

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
        public DateTime Joined { get; set; }

        [BsonElement("Verified")]
        [JsonProperty("Verified")]
        [BsonRepresentation(BsonType.String)]
        public string Verified { get; set; }

        [BsonElement("Role")]
        [JsonProperty("Role")]
        [BsonRepresentation(BsonType.Int32)]
        public Role Role { get; set; }

        [BsonElement("Active")]
        [JsonProperty("Active")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool Active { get; set; }

        [BsonElement("Suspension")]
        [JsonProperty("Suspension")]
        public Suspension Suspension { get; set; }

        public UserBase(string Username, string Password, string Email, bool Notification, string Verified, DateTime Joined, Role Role) {
            //this.Id = ObjectId.GenerateNewId().ToString();
            this.Username = Username;
            this.Password = Password;
            this.Email = Email;
            this.Notification = Notification;
            this.Verified = Verified;
            this.Joined = Joined;
            this.Role = Role;
            this.Active = true;
        }

        public UserBase(string Username, string Password, string Email, bool Notification, string Verified, DateTime Joined, Role Role, Suspension Suspension) {
            //this.Id = ObjectId.GenerateNewId().ToString();
            this.Username = Username;
            this.Password = Password;
            this.Email = Email;
            this.Notification = Notification;
            this.Verified = Verified;
            this.Joined = Joined;
            this.Role = Role;
            this.Active = true;
            this.Suspension = Suspension;
        }

        public abstract bool ValidateRole();
    }

    [BsonDiscriminator("User")]
    public class User : UserBase {

        [BsonConstructor("Username", "Password", "Email", "Notification", "Verified", "Joined", "Role")]
        public User(string Username, string Password, string Email, bool Notification, string Verified, DateTime Joined, Role Role) : 
        base(Username, Password, Email, Notification, Verified, Joined, Role) {}

        [BsonConstructor("Username", "Password", "Email", "Notification", "Verified", "Joined", "Role", "Suspension")]
        public User(string username, string password, string Email, bool notification, string verified, DateTime joined, Role role, Suspension suspension) : 
        base(username, password, Email, notification, verified, joined, role, suspension) {}

        public override bool ValidateRole() {
            bool proof = Role >= Role.Moderator;

            proof = proof && Suspension.Duration < DateTime.Now;
            
            return proof;
        }
    }
    
    /// <summary>
    /// Instantiated class of IUserDocument
    /// </summary>
    /// <remarks>
    /// The document doesn't preserves it's initial fields type/value structure
    /// </remarks>
    public class UserDocument : BsonDocument, IBsonUserDocument {
        public BsonDocument User { get; set; }

        public UserDocument(User User) {
            this.User = User.ToBsonDocument();
        }

        /// <summary>
        /// Returns a functor of a document mapping between a bson field and another bson field
        /// </summary>
        /// <param name="key">Identity of the field</param>
        /// <param name="functor">Function to morph the bson value</param>
        /// <returns>User object with a field mapped by the functor</returns>
        public UserDocument Functor<B>(string key, Func<BsonValue, B> functor) where B : BsonValue {
            int i = User.IndexOfName(key);
            B b = functor(User.GetValue(i));

            User.Set(i, b);

            return this;
        }
    }
}