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
using Models;

namespace Services {
    public class UserService {

        private IMongoCollection<BsonUser> _users { get; }

        public UserService (IUserstoreDatabaseSettings settings) {
            MongoClient client = new MongoClient(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<BsonUser>(settings.UsersCollectionName);
        }

        /// <summary>
        /// Returns the Users enumerable collection
        /// </summary>
        /// <returns>Users enumerable collection</returns>
        public IEnumerable<BsonUser> GetAll () => _users.Find(user => true).ToEnumerable();

        /// <summary>
        /// Finds a User from an enumerable collection by username
        /// </summary>
        /// <param name="id">Id of the user to find</param>
        /// <returns>User object or null</returns>
        public BsonUser GetUser (string id) => _users.Find<BsonUser>(user => user.Id == id).FirstOrDefault();

        public BsonUser CreateUser (BsonUser user) {
            _users.InsertOne(user);
            return user;
        }

        public BsonUser ReplaceUser (string id, BsonUser newuser) {
            _users.ReplaceOne(user => user.Id == id, newuser);
            return newuser;
        }

        public bool Moderate (BsonUser user) {
            bool proof = user.role >= Role.Moderator;
            proof = proof && user.suspended.duration == null;
            
            return proof;
        }

        public BsonUser SuspendUser (string id, string reason, DateTime duration) {
            UpdateDefinitionBuilder<BsonUser> bupdate = new UpdateDefinitionBuilder<BsonUser>();
            UpdateDefinition<BsonUser> update = bupdate.AddToSet("reason", reason).AddToSet("duration", duration);

            return _users.FindOneAndUpdate<BsonUser>(user => user.Id == id, update);
        }

        public BsonUser RemoveUser (string id) => _users.FindOneAndDelete(user => user.Id == id);

        /// <summary>
        /// Morph a bson field by bsontype and by value as an argument of a function
        /// </summary>
        /// <param name="id">Id of the user to find</param>
        /// <param name="key">Key of the field to change</param>
        /// <param name="functor">Endomorphic mapping between the type of the field</param>
        /// <typeparam name="B">BsonValue</typeparam>
        /// <returns></returns>
        public BsonUser ChangeField<B> (string id, string key, Func<BsonValue, BsonValue> functor) {
            BsonUserDocument doc = new BsonUserDocument(this.GetUser(id));
            doc = doc.functor<B>(key, functor);
            return BsonSerializer.Deserialize<BsonUser>(doc.user);
        }

    }
}