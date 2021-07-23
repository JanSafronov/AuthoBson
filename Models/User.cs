using System;
using System.Collections;
using System.Collections.Generic;
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

    public enum UserType { GenericUser, Senior, Moderator, Administrator }


    
    public interface IBsonUser {

        [BsonElement("username")]
        BsonString username { get; set; }

        [BsonElement("password")]
        BsonString password { get; set; }

        [BsonElement("email")]
        BsonString email { get; set; }

        [BsonElement("notification")]
        BsonBoolean notification { get; set; }

        [BsonElement("usertype")]
        UserType usertype { get; set; }

        /// <summary>
        /// Functor mapping between fields and preserving the initial type
        /// </summary>
        /// <param name="id">Identity of the field</param>
        /// <param name="functor">Pattern of mapping</param>
        /// <returns>User object with a field mapped by the functor</returns>
        IBsonUser functor<B> (Func<B, BsonValue> functor);
    }

    public interface IBsonUserDocument : IBsonSerializer {
        
        /// <summary>
        /// Functor mapping between fields and preserving the initial type
        /// </summary>
        /// <param name="id">Identity of the field</param>
        /// <param name="functor">Pattern of mapping</param>
        /// <returns>User object with a field mapped by the functor</returns>
        /// IBsonUser functor (Identity id, Func<BsonString, BsonValue> functor);
    }

    public class IFunctorBsonUser : IBsonUser {

        /// <summary>
        /// Functor mapping between fields and preserving the initial type
        /// </summary>
        /// <param name="id">Identity of the field</param>
        /// <param name="functor">Pattern of mapping</param>
        /// <returns>User object with a field mapped by the functor</returns>
        IBsonUser functor (Func<BsonString, BsonValue> functor) {
            email.
        }
    }

    public class Test {
        
    }
}