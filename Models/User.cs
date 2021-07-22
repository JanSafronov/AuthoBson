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

    public enum UserT
    {
        GenericUser,
        Senior,
        Moderator,
        Administrator
    }

    public enum Identity {
        username,
        password,
        email,
        notification,
        usertype
    }

    // <summary>
    /// Abstract class of user fields
    /// </summary>
    /// <typeparam name="T">Data type of an identified field</typeparam>
    /// <example>
    /// (username, string) represents a tuple of such field
    /// </example>
    public class Field<T> {
        public Identity id { get; set; }
        
        public T value { get; set; }

        public Field (T value) {
            this.value = value;
        }

        public Field (T value, Identity id) {
            this.id = id;
            this.value = value;
        }
    }

    public interface IBsonUser {

        BsonString username { get; set; }

        BsonString password { get; set; }

        BsonString email { get; set; }

        BsonBoolean notification { get; set; }

        UserT usertype { get; set; }

        /// <summary>
        /// Functor mapping between fields and preserving the initial type
        /// </summary>
        /// <param name="id">Identity of the field</param>
        /// <param name="functor">Pattern of mapping</param>
        /// <returns>User object with a field mapped by the functor</returns>
        IBsonUser functor (Identity id, Func<BsonString, BsonValue> functor) {
            
            id = BsonString.Create(functor(this.username));
            return this;
        }


    }

    public interface IBsonUserDocument : IBsonSerializer {

    }


    public class Moderator : IBsonUser {
        public BsonString username { get; set; }

        public BsonString password { get; set; }

        public BsonString email { get; set; }

        public BsonBoolean notification { get; set; }

        public UserT usertype { get; set; }

        public GenericUser (string username, string password, string email, bool notification) {
            this.username.value = username;
            this.password.value = password;
            this.email.value = email;
            this.notification.value = notification;
            this.usertype = UserT.GenericUser;
        }
    }

    public class Senior : GenericUser {
        public Senior (string username, string password, string email, bool notification) {
            this.username.value = username;
            this.password.value = password;
            this.email.value = email;
            this.notification.value = notification;
            this.usertype = UserT.GenericUser;
        }
    }

    public class Test {
        
    }
}