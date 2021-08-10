using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Driver.Core.Clusters;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Driver.GeoJsonObjectModel.Serializers;
using MongoDB.Driver.Core.Connections;
using MongoDB.Driver.Core.Events;
using MongoDB.Driver.Core.Events.Diagnostics;
using MongoDB.Driver.Core.Operations;
using MongoDB.Driver.Encryption;
using MongoDB.Driver.Linq;
using AuthoBson.Models;

namespace AuthoBson.Services {

    public abstract class UserServiceBase {
        private IMongoCollection<User> _users { get; set; }

        public UserServiceBase(IUserstoreDatabaseSettings settings) {
            MongoClient client = new MongoClient(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UsersCollectionName);
        }

        public UserServiceBase(IUserstoreDatabase settings) {
            MongoClient client = new MongoClient();
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UsersCollectionName);
        }

        public abstract IEnumerable<User> GetAll();

        public abstract User GetUser (string id);

        public abstract User CreateUser (User user);

        public abstract User ReplaceUser (string id, User newuser);

        public abstract User SuspendUser (string id, Suspension suspension);

        public abstract User RemoveUser (string id);

        public abstract User ChangeField<B> (string id, string key, Func<BsonValue, BsonValue> functor) where B : BsonValue;
    }

    public class UserService : UserServiceBase {

        private IMongoCollection<User> _users { get; set; }

        public UserService(IUserstoreDatabaseSettings settings) :
        base(settings) {}

        public UserService(IUserstoreDatabase settings) :
        base(settings) {}

        /// <summary>
        /// Returns the Users enumerable collection
        /// </summary>
        /// <returns>Users enumerable collection</returns>
        public override IEnumerable<User> GetAll () => _users.Find(user => true).ToEnumerable();

        /// <summary>
        /// Finds a User from an enumerable collection by username
        /// </summary>
        /// <param name="id">Id of the user to find</param>
        /// <returns>User object or null</returns>
        public override User GetUser (string id) => _users.Find<User>(user => user.Id == id).FirstOrDefault();

        public override User CreateUser (User user) {
            _users.InsertOne(user);
            return user;
        }

        public override User ReplaceUser (string id, User newuser) {
            _users.ReplaceOne(user => user.Id == id, newuser);
            return newuser;
        }

        public override User SuspendUser (string id, Suspension suspension) {
            UpdateDefinitionBuilder<User> bupdate = new UpdateDefinitionBuilder<User>();
            UpdateDefinition<User> update = bupdate.AddToSet("suspension", suspension);

            return _users.FindOneAndUpdate<User>(user => user.Id == id, update);
        }

        public override User RemoveUser (string id) => _users.FindOneAndDelete(user => user.Id == id);

        /// <summary>
        /// Morph a bson field by bsontype and by value as an argument of a function
        /// </summary>
        /// <param name="id">Id of the user to find</param>
        /// <param name="key">Key of the field to change</param>
        /// <param name="functor">Endomorphic mapping between the type of the field</param>
        /// <typeparam name="B">BsonValue</typeparam>
        /// <returns></returns>
        public override User ChangeField<B> (string id, string key, Func<BsonValue, BsonValue> functor) {
            UserDocument doc = new UserDocument(this.GetUser(id));
            doc = doc.functor<B>(key, functor);
            return BsonSerializer.Deserialize<User>(doc.user);
        }
    }

    public class OpenUserService : UserService {
        public IMongoCollection<User> _users { get; set; }

        public OpenUserService(IUserstoreDatabaseSettings settings) : 
        base(settings) {}
    }
}