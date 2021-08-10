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

    [BsonDiscriminator("Suspension")]
    public class Suspension : BsonValue {

        [BsonElement("reason")]
        [JsonProperty("reason")]
        [BsonRepresentation(BsonType.String)]
        public String reason { get; set; }

        [BsonElement("duration")]
        [JsonProperty("duration")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime duration { get; set; }

        [BsonConstructor("reason", "duration")]
        public Suspension(string reason, DateTime duration) {
            this.reason = reason;
            this.duration = duration;
        }

        public override BsonType BsonType => this.BsonType;

        public override int CompareTo(BsonValue value) => value.CompareTo(this);

        public override bool Equals(object obj)
        {
            
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            return (obj.Equals(this));
        }
        
        public override int GetHashCode() => this.reason.GetHashCode() ^ this.duration.GetHashCode();
    }

    public interface IGenericUser {

        string Id { get; }

        string username { get; set; }

        string password { get; set; }

        string email { get; set; }

        Boolean notification { get; set; }

        DateTime joined { get; }

        Role role { get; set; }

        string verified { get; set; }

        Suspension suspension { get; set; }

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
        UserDocument functor<B>(string key, Func<BsonValue, B> functor) where B : BsonValue;
    }

    /// <summary>
    /// Abstract implementation of generic user interface
    /// </summary>
    /// <remarks>
    /// Not recommended for documentless use due to bson documents and morphism incapabilities
    /// </remarks>
    [BsonDiscriminator("BsonUser")]
    [BsonKnownTypes(typeof(User))]
    public abstract class GenericUser : IGenericUser {

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
        public Suspension suspension { get; set; }

        public GenericUser(string username, string password, string email, bool notification, DateTime joined, Role role) {
            this.username = username;
            this.password = password;
            this.email = email;
            this.notification = notification;
            this.joined = joined;
            this.role = role;
        }

        public GenericUser(string username, string password, string email, bool notification, DateTime joined, Role role, Suspension suspension) {
            this.username = username;
            this.password = password;
            this.email = email;
            this.notification = notification;
            this.joined = joined;
            this.role = role;
            this.suspension = suspension;
        }

        public abstract bool ValidateRole();
    }

    [BsonDiscriminator("User")]
    public class User : GenericUser {

        [BsonConstructor("username", "password", "email", "notification", "joined", "role")]
        public User(string username, string password, string email, bool notification, DateTime joined, Role role) : 
        base(username, password, email, notification, joined, role) {}

        [BsonConstructor("username", "password", "email", "notification", "joined", "role", "suspension")]
        public User(string username, string password, string email, bool notification, DateTime joined, Role role, Suspension suspension) : 
        base(username, password, email, notification, joined, role, suspension) {}

        public override bool ValidateRole() {
            bool proof = role >= Role.Moderator;

            proof = proof && suspension.duration < DateTime.Now;
            
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
        public UserDocument functor<B>(string key, Func<BsonValue, B> functor) where B : BsonValue {
            int i = user.IndexOfName(key);
            B b = functor(user.GetValue(i));

            user.InsertAt(i, new BsonElement(key, b));

            return this;
        }
    }
}