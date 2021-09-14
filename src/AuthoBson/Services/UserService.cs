using AuthoBson.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AuthoBson.Services
{

    public class UserService {

        private IMongoCollection<User> Users { get; set; }
        
        public UserService(IUserstoreDatabaseSettings settings) {
            MongoClient client = new(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            Users = database.GetCollection<User>(settings.UsersCollectionName);
        }

        public UserService(IUserstoreDatabase settings) {
            MongoClient client = new();
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            Users = database.GetCollection<User>(settings.UsersCollectionName);
        }

        /// <summary>
        /// Returns the Users enumerable collection
        /// </summary>
        /// <returns>Users enumerable collection</returns>
        public IEnumerable<User> GetAll () => Users.Find(User => true).ToEnumerable();

        /// <summary>
        /// Finds a User from an enumerable collection by username
        /// </summary>
        /// <param name="id">Id of the user to find</param>
        /// <returns>User object or null</returns>
        public User GetUser (string Id) => Users.Find<User>(User => User.Id == Id).FirstOrDefault();

        public User CreateUser (User User) {
            Users.InsertOne(User);
            return User;
        }

        public User ReplaceUser (string Id, User newUser) {
            Users.ReplaceOne(User => User.Id == Id, newUser);
            return newUser;
        }

        public User SuspendUser (string Id, Suspension Suspension) {
            UpdateDefinitionBuilder<User> bupdate = new();
            UpdateDefinition<User> update = bupdate.AddToSet("Suspension", Suspension);

            return Users.FindOneAndUpdate<User>(user => user.Id == Id, update);
        }

        public User RemoveUser (string Id) => Users.FindOneAndDelete(User => User.Id == Id);

        /// <summary>
        /// Morph a bson field by bsontype and by value as an argument of a function
        /// </summary>
        /// <param name="Id">Id of the user to find</param>
        /// <param name="key">Key of the field to change</param>
        /// <param name="functor">Endomorphic mapping between the type of the field</param>
        /// <typeparam name="B">BsonValue</typeparam>
        /// <returns>Morphed user</returns>
        public User ChangeField<B> (string Id, string key, Func<BsonValue, B> functor) where B : BsonValue {
            UserDocument doc = new(this.GetUser(Id));
            doc = doc.Functor<B>(key, functor);
            return BsonSerializer.Deserialize<User>(doc.User);
        }
    }
}