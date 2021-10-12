using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
using AuthoBson.Models.Templates;
using AuthoBson.Shared;
using AuthoBson.Shared.Data.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AuthoBson.Models {

    public enum Role { Generic, Senior, Moderator, Administrator }

    public interface ISuspension {
        [BsonElement("Reason")]
        [JsonProperty("Reason")]
        [BsonRepresentation(BsonType.String)]
        String Reason { get; set; }

        [BsonElement("Duration")]
        [JsonProperty("Duration")]
        //[BsonRepresentation(BsonType.DateTime)]
        DateTime Duration { get; set; }
    }

    [BsonDiscriminator("Suspension")]
    public class Suspension : ISuspension {

        [BsonElement("Reason")]
        [JsonProperty("Reason")]
        [BsonRepresentation(BsonType.String)]
        public String Reason { get; set; }

        [BsonElement("Duration")]
        [JsonProperty("Duration")]
        //[BsonRepresentation(BsonType.DateTime)]
        public DateTime Duration { get; set; }

        [BsonConstructor("Reason", "Duration")]
        public Suspension(string Reason, DateTime Duration) {
            this.Reason = Reason;
            this.Duration = Duration;
        }

        public Suspension() {
            this.Reason = "Default";
            this.Duration = DateTime.Now;
        }
    }

    [Messaging]
    public interface IUser : IModelBase {

        [BsonElement("Username")]
        [JsonProperty("Username")]
        [BsonRepresentation(BsonType.String)]
        string Username { get; set; }

        [BsonElement("Password")]
        [JsonProperty("Password")]
        [BsonRepresentation(BsonType.String)]
        string Password { get; set; }

        [BsonElement("Email")]
        [JsonProperty("Email")]
        [BsonRepresentation(BsonType.String)]
        string Email { get; set; }

        [BsonElement("Notification")]
        [JsonProperty("Notification")]
        [BsonRepresentation(BsonType.Boolean)]
        Boolean Notification { get; set; }

        [BsonElement("Joined")]
        [JsonProperty("Joined")]
        //[BsonRepresentation(BsonType.DateTime)]
        DateTime Joined { get; }

        [BsonElement("Role")]
        [JsonProperty("Role")]
        [BsonRepresentation(BsonType.Int32)]
        Role Role { get; set; }

        [BsonElement("Verified")]
        [JsonProperty("Verified")]
        [BsonRepresentation(BsonType.String)]
        string Verified { get; set; }

        [BsonElement("Active")]
        [JsonProperty("Active")]
        [BsonRepresentation(BsonType.Boolean)]
        bool Active { get; set; }

        [BsonElement("Suspension")]
        [JsonProperty("Suspension")]
        Suspension Suspension { get; set; }

        [BsonElement("Salt")]
        [JsonProperty("Salt")]
        [BsonRepresentation(BsonType.String)]
        string Salt { get; set; }

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
        [BsonRepresentation(BsonType.ObjectId)]
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
        //[BsonRepresentation(BsonType.DateTime)]
        public DateTime Joined { get; set; }

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
        public string Salt { get; set; }

        public UserBase(string Username, string Password, string Email, bool Notification, DateTime Joined, Role Role, string Verified) {
            this.Id = ObjectId.GenerateNewId().ToString();
            this.Username = Username;
            this.Password = Password;
            this.Email = Email;
            this.Notification = Notification;
            this.Joined = Joined;
            this.Role = Role;
            this.Verified = Verified;
            this.Active = true;
        }

        public UserBase(string Username, string Password, string Email, bool Notification, DateTime Joined, Role Role, string Verified, Suspension suspension) {
            this.Id = ObjectId.GenerateNewId().ToString();
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

        public abstract bool ValidateRole();
    }

    [BsonDiscriminator("User")]
    public class User : UserBase, IUser {

        [BsonConstructor("Username", "Password", "Email", "Notification", "Joined",  "Role", "Verified", "Suspension")]
        public User(string Username, string Password, string Email, bool Notification, string Verified, DateTime Joined, Role Role, Suspension Suspension) : 
        base(Username, Password, Email, Notification, Joined, Role, Verified, Suspension) {}

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