using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Enumeration;
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
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Driver.Core.Authentication;
using MongoDB.Driver.Core.Compression;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Encryption;

namespace AuthoBson.Models {

    public enum Role { Generic, Senior, Moderator, Administrator }

    public struct BsonSuspended {

        [BsonElement("reason")]
        [JsonProperty("reason")]
        [BsonRepresentation(BsonType.String)]
        public String reason { get; set; }

        [BsonElement("duration")]
        [JsonProperty("duration")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime duration { get; set; }

        [BsonConstructor("reason", "duration")]
        public BsonSuspended(string reason, DateTime duration) {
            this.reason = reason;
            this.duration = duration;
        }
    }

    public interface IBsonUser {

        string Id { get; }

        string username { get; set; }

        string password { get; set; }

        string email { get; set; }

        Boolean notification { get; set; }

        DateTime joined { get; }

        Role role { get; set; }

        string verified { get; set; }

        BsonSuspended suspended { get; set; }

        bool ValidateRole();
    }

    public interface IBsonUserDocument {

        BsonDocument user { get; set; }
        
        /// <summary>
        /// Functor mapping between fields and preserving the initial type
        /// </summary>
        /// <param name="key">Identity of the field</param>
        /// <param name="functor">Pattern of mapping</param>
        /// <returns>User object with a field mapped by the functor</returns>
        UserDocument functor<B>(string key, Func<BsonValue, BsonValue> functor) where B : BsonValue;
    }

    /// <summary>
    /// Abstract implementation of generic user interface
    /// </summary>
    /// <remarks>
    /// Not recommended for documentless use due to bson documents and morphism incapabilities
    /// </remarks>
    [BsonDiscriminator("BsonUser")]
    [BsonKnownTypes(typeof(User))]
    public abstract class BsonUser : IBsonUser {

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; }

        [BsonElement("username")]
        [JsonProperty("username")]
        [BsonRepresentation(BsonType.String)]
        public string username { get; set; }

        [BsonElement("password")]
        [JsonProperty("password")]
        [BsonRepresentation(BsonType.String)]
        public string password { get; set; }

        [BsonElement("email")]
        [JsonProperty("email")]
        [BsonRepresentation(BsonType.String)]
        public string email { get; set; }

        [BsonElement("notification")]
        [JsonProperty("notification")]
        [BsonRepresentation(BsonType.Boolean)]
        public Boolean notification { get; set; }

        [BsonElement("joined")]
        [JsonProperty("joined")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime joined { get; set; }

        [BsonElement("verified")]
        [JsonProperty("verified")]
        [BsonRepresentation(BsonType.String)]
        public string verified { get; set; }

        [BsonElement("role")]
        [JsonProperty("role")]
        [BsonRepresentation(BsonType.Int32)]
        public Role role { get; set; }

        [BsonElement("suspended")]
        [JsonProperty("suspended")]
        //[BsonRepresentation(BsonType.String)]
        public BsonSuspended suspended { get; set; }

        /*/// <summary>
        /// User constructor for initialization
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <param name="notification"></param>
        /// <param name="joined"></param>
        /// <param name="role"></param>
        [BsonConstructor("username", "password", "email", "notification", "joined", "role")]
        public BsonUser(string username, string password, string email, bool notification, DateTime joined, Role role) {
            this.username = username;
            this.password = password;
            this.email = email;
            this.notification = notification;
            this.joined = joined;
            this.role = role;
        }

        /// <summary>
        /// User constructor for 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <param name="notification"></param>
        /// <param name="joined"></param>
        /// <param name="role"></param>
        /// <param name="suspended"></param>
        [BsonConstructor("username", "password", "email", "notification", "joined", "role", "suspended")]
        public BsonUser(string username, string password, string email, bool notification, DateTime joined, Role role, BsonSuspended suspended) {
            this.username = username;
            this.password = password;
            this.email = email;
            this.notification = notification;
            this.joined = joined;
            this.role = role;
            this.suspended = suspended;
        }*/

        public abstract bool ValidateRole();
    }

    [BsonDiscriminator("User")]
    public class User : BsonUser {

        //[BsonConstructor("username", "password", "email", "notification", "joined", "role")]
        //public User(string username, string password, string email, bool notification, DateTime joined, Role role) : 
        //base(username, password, email, notification, joined, role) {}

        [BsonConstructor("username", "password", "email", "notification", "joined", "role")]
        protected User(string username, string password, string email, bool notification, DateTime joined, Role role) {
            this.username = username;
            this.password = password;
            this.email = email;
            this.notification = notification;
            this.joined = joined;
            this.role = role;
        }

        [BsonConstructor("username", "password", "email", "notification", "joined", "role", "suspended")]
        protected User(string username, string password, string email, bool notification, DateTime joined, Role role, BsonSuspended suspended) {
            this.username = username;
            this.password = password;
            this.email = email;
            this.notification = notification;
            this.joined = joined;
            this.role = role;
            this.suspended = suspended;
        }

        //[BsonConstructor("username", "password", "email", "notification", "joined", "role", "suspended")]
        //public User(string username, string password, string email, bool notification, DateTime joined, Role role, BsonSuspended suspended) : 
        //base(username, password, email, notification, joined, role, suspended) {}

        public override bool ValidateRole() {
            bool proof = role >= Role.Moderator;

            proof = proof && suspended.duration < DateTime.Now;
            
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
        public BsonDocument user { get; set; }

        public UserDocument(User user) {
            this.user = user.ToBsonDocument();
        }

        /// <summary>
        /// Returns a functor of a document mapping between a bson field and another bson field
        /// </summary>
        /// <param name="key">Identity of the field</param>
        /// <param name="functor">Function to morph the bson value</param>
        /// <returns>User object with a field mapped by the functor</returns>
        public UserDocument functor<B>(string key, Func<BsonValue, BsonValue> functor) where B : BsonValue {
            
            // Get the bson value by position
            int i = user.IndexOfName(key);
            BsonValue b = functor(user.GetValue(i));

            // Deserialize a value from BsonValue -> B -> BsonValue
            B newb = BsonSerializer.Deserialize<B>(b.ToJson());
            BsonValue newbB = BsonValueSerializer.Instance.ToBsonValue(newb);

            user.InsertAt(i, new BsonElement(key, newbB));

            return this;
        }
    }
}