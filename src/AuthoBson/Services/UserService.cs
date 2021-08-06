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
using MongoDB.Driver.Core.Connections;
using MongoDB.Driver.Core.Events;
using MongoDB.Driver.Core.Events.Diagnostics;
using MongoDB.Driver.Core.Operations;
using MongoDB.Driver.Encryption;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Driver.Linq;
using AuthoBson.Models;

namespace AuthoBson.Services {
    public class UserService {

        private IMongoCollection<User> _users { get; }

        public UserService (IUserstoreDatabaseSettings settings) {
            MongoClient client = new MongoClient(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UsersCollectionName);
        }

        /// <summary>
        /// Returns the Users enumerable collection
        /// </summary>
        /// <returns>Users enumerable collection</returns>
        public IEnumerable<User> GetAll () => _users.Find(user => true).ToEnumerable();

        /// <summary>
        /// Finds a User from an enumerable collection by username
        /// </summary>
        /// <param name="id">Id of the user to find</param>
        /// <returns>User object or null</returns>
        public User GetUser (string id) => _users.Find<User>(user => user.Id == id).FirstOrDefault();

        public User CreateUser (User user) {
            _users.InsertOne(user);
            return user;
        }

        public User ReplaceUser (string id, User newuser) {
            _users.ReplaceOne(user => user.Id == id, newuser);
            return newuser;
        }

        public User SuspendUser (string id, string reason, DateTime duration) {
            UpdateDefinitionBuilder<User> bupdate = new UpdateDefinitionBuilder<User>();
            UpdateDefinition<User> update = bupdate.AddToSet("reason", reason).AddToSet("duration", duration);

            return _users.FindOneAndUpdate<User>(user => user.Id == id, update);
        }

        public User RemoveUser (string id) => _users.FindOneAndDelete(user => user.Id == id);

        /// <summary>
        /// Morph a bson field by bsontype and by value as an argument of a function
        /// </summary>
        /// <param name="id">Id of the user to find</param>
        /// <param name="key">Key of the field to change</param>
        /// <param name="functor">Endomorphic mapping between the type of the field</param>
        /// <typeparam name="B">BsonValue</typeparam>
        /// <returns></returns>
        public User ChangeField<B> (string id, string key, Func<BsonValue, BsonValue> functor) where B : BsonValue {
            UserDocument doc = new UserDocument(this.GetUser(id));
            doc = doc.functor<B>(key, functor);
            return BsonSerializer.Deserialize<User>(doc.user);
        }
    }
}