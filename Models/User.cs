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

namespace Models {

    public enum Role { Generic, Senior, Moderator, Administrator }

    public struct Suspended {
        public BsonString reason { get; set; }

        public BsonDateTime duration { get; set; }

        public Suspended (string reason, DateTime duration) {
            this.reason = reason;
            this.duration = duration;
        }
    }

    public interface IBsonUser {

        BsonString Id { get; }

        BsonString username { get; set; }

        BsonString password { get; set; }

        BsonString email { get; set; }

        BsonBoolean notification { get; set; }

        BsonDateTime joined { get; }

        Role role { get; set; }

        BsonString verified { get; set; }
    }

    
    public interface IBsonUserDocument {

        BsonDocument user { get; set; }
        
        /// <summary>
        /// Functor mapping between fields and preserving the initial type
        /// </summary>
        /// <param name="key">Identity of the field</param>
        /// <param name="functor">Pattern of mapping</param>
        /// <returns>User object with a field mapped by the functor</returns>
        BsonUserDocument functor<B> (string key, Func<BsonValue, BsonValue> functor);
    }

    /// <summary>
    /// Abstract implementation of generic user interface
    /// </summary>
    /// <remarks>
    /// Not recommended for documentless use due to bson documents and morphism incapabilities
    /// </remarks>
    public abstract class GenericBsonUser : IBsonUser {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public BsonString Id { get; }

        [BsonElement("username")]
        [JsonProperty("username")]
        [BsonRepresentation(BsonType.String)]
        public BsonString username { get; set; }

        [BsonElement("password")]
        [JsonProperty("password")]
        [BsonRepresentation(BsonType.String)]
        public BsonString password { get; set; }

        [BsonElement("email")]
        [JsonProperty("email")]
        [BsonRepresentation(BsonType.String)]
        public BsonString email { get; set; }

        [BsonElement("notification")]
        [JsonProperty("notification")]
        [BsonRepresentation(BsonType.Boolean)]
        public BsonBoolean notification { get; set; }

        [BsonElement("joined")]
        [JsonProperty("joined")]
        [BsonRepresentation(BsonType.DateTime)]
        public BsonDateTime joined { get; }

        [BsonElement("verified")]
        [JsonProperty("verified")]
        [BsonRepresentation(BsonType.String)]
        public BsonString verified { get; set; }

        [BsonElement("role")]
        [JsonProperty("role")]
        [BsonRepresentation(BsonType.String)]
        public Role role { get; set; }

        [BsonConstructor("username", "password", "email", "notification", "hoined", "role")]
        protected GenericBsonUser (string username, string password, string email, bool notification, DateTime joined, Role role) {
            this.username = username;
            this.password = password;
            this.email = email;
            this.notification = notification;
            this.joined = joined;
            this.role = role;
        }
    }

    public class BsonUser : GenericBsonUser {
        public Suspended suspended { get; set; }

        [BsonConstructor("username", "password", "email", "notification", "role")]
        public BsonUser (string username, string password, string email, bool notification, DateTime joined, Role role) : 
        base (username, password, email, notification, joined, role) {}
    }
    
    /// <summary>
    /// Instantiated class of IBsonUserDocument
    /// </summary>
    /// <remarks>
    /// The document doesn't preserves it's initial fields type/value structure
    /// </remarks>
    public class BsonUserDocument : IBsonUserDocument {
        public BsonDocument user { get; set; }

        public BsonUserDocument(BsonUser user) {
            this.user = user.ToBsonDocument();
        }

        /// <summary>
        /// Returns a functor of a document mapping between a bson field and another bson field
        /// </summary>
        /// <param name="key">Identity of the field</param>
        /// <param name="functor">Function to morph the bson value</param>
        /// <returns>User object with a field mapped by the functor</returns>
        public BsonUserDocument functor<B> (string key, Func<BsonValue, BsonValue> functor) {

            // Return the document if the type input isn't of the BsonValue
            if (typeof(B) == typeof(BsonValue))
                return this;
            
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