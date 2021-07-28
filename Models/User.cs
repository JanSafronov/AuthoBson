using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Enumeration;
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

namespace Models {

    public enum Role { Generic, Senior, Moderator, Administrator }

    public interface IBsonUser {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        BsonString Id { get; set; }

        [BsonElement("username")]
        [BsonRepresentation(BsonType.String)]
        BsonString username { get; set; }

        [BsonElement("password")]
        [BsonRepresentation(BsonType.String)]
        BsonString password { get; set; }

        [BsonElement("email")]
        [BsonRepresentation(BsonType.String)]
        BsonString email { get; set; }

        [BsonElement("notification")]
        [BsonRepresentation(BsonType.Boolean)]
        BsonBoolean notification { get; set; }

        [BsonElement("role")]
        [BsonRepresentation(BsonType.String)]
        Role role { get; set; }
    }

     

    public interface IBsonGenericUser : IBsonUser {
        [BsonElement("password")]
        [BsonRepresentation(BsonType.String)]
        BsonString reason { get; set; }

        [BsonElement("email")]
        [BsonRepresentation(BsonType.DateTime)]
        BsonDateTime duration { get; set; }
    }

    public abstract class BsonUserDocument {

        private BsonUser user { get; set; }
        
        /// <summary>
        /// Functor mapping between fields and preserving the initial type
        /// </summary>
        /// <param name="key">Identity of the field</param>
        /// <param name="functor">Pattern of mapping</param>
        /// <returns>User object with a field mapped by the functor</returns>
        public abstract BsonUserDocument functor<B> (string key, Func<BsonValue, BsonValue> functor);
    }

    /// <summary>
    /// Instantiated class of IBsonUser
    /// </summary>
    /// <remarks>
    /// Not recommended for documentless use due to bson documents and morphism incapabilities
    /// </remarks>
    public class BsonUser : IBsonUser {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public BsonString Id { get; set; }

        [BsonElement("username")]
        public BsonString username { get; set; }

        [BsonElement("password")]
        public BsonString password { get; set; }

        [BsonElement("email")]
        public BsonString email { get; set; }

        [BsonElement("notification")]
        public BsonBoolean notification { get; set; }

        [BsonElement("role")]
        public Role role { get; set; }

        [BsonConstructor("username", "password", "email", "notification", "role")]
        public BsonUser (string username, string password, string email, bool notification, Role role) {
            this.username = username;
            this.password = password;
            this.email = email;
            this.notification = notification;
            this.role = role;
        }
    }
    
    /// <summary>
    /// Instantiated class of IBsonUserDocument
    /// </summary>
    /// <remarks>
    /// The document doesn't preserves it's initial fields type/value structure
    /// </remarks>
    public class BsonUserDocumen : BsonUserDocument {
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
        public override BsonUserDocument functor<B> (string key, Func<BsonValue, BsonValue> functor) {

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