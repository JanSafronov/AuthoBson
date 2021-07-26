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
using MongoDB.Driver.Linq;

namespace Models {

    public enum Role { Generic, Senior, Moderator, Administrator }

    public interface IBsonUser {

        [BsonElement("username")]
        BsonString username { get; set; }

        [BsonElement("password")]
        BsonString password { get; set; }

        [BsonElement("email")]
        BsonString email { get; set; }

        [BsonElement("notification")]
        BsonBoolean notification { get; set; }

        [BsonElement("role")]
        Role role { get; set; }
    }

    public interface IBsonUserDocument {

        BsonDocument user { get; set; }
        
        /// <summary>
        /// Functor mapping between fields and preserving the initial type
        /// </summary>
        /// <param name="key">Identity of the field</param>
        /// <param name="functor">Pattern of mapping</param>
        /// <returns>User object with a field mapped by the functor</returns>
        BsonUserDocument functor (string key, Func<BsonValue, BsonValue> functor);
    }

    /// <summary>
    /// Instantiated class of IBsonUser
    /// </summary>
    /// <remarks>
    /// Not recommended due to bson documents and morphism incapabilities
    /// </remarks>
    public class BsonUser : IBsonUser {

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
    }
    
    /// <summary>
    /// Instantiated class of IBsonUserDocument
    /// </summary>
    /// <remarks>
    /// The document doesn't preserves it's initial fields type/value structure
    /// </remarks>
    public class BsonUserDocument : IBsonUserDocument {
        public BsonDocument user { get; set; }

        /// <summary>
        /// Functor mapping between fields and preserving the initial type
        /// </summary>
        /// <param name="key">Identity of the field</param>
        /// <param name="functor">Function to morph the bson value</param>
        /// <returns>User object with a field mapped by the functor</returns>
        public BsonUserDocument functor<BsonType> (string key, Func<BsonValue, BsonValue> functor) {
            if (typeof(BsonType) != typeof(BsonValue))
                return this;

            int i = user.IndexOfName(key);
            BsonValue b = functor(user.GetValue(i));

            user.InsertAt(i, new BsonElement(key, BsonSerializer.Deserialize<T>(b)));
        }
    }

    public class Test {
        
    }
}