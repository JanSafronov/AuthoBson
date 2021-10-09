using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Security.Authentication;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication.ExtendedProtection;
using AuthoBson.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using AuthoBson.Models.Templates;
using AuthoBson.Services.Security;
using AuthoBson.Shared.Data.Models;

namespace AuthoBson.Services
{

    public class UserService {

        private IMongoCollection<User> Users { get; set; }

        private IUserTemplate Template { get; set; }
        
        public UserService(IUserstoreDatabaseSettings settings, IUserTemplate template) {
            MongoClient client = new(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            Users = database.GetCollection<User>(settings.UsersCollectionName);

            Template = template;
        
        }

        public UserService(IUserstoreDatabase settings, IUserTemplate template) {
            MongoClient client = new();
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            Users = database.GetCollection<User>(settings.UsersCollectionName);

            Template = template;
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
            if (Template.IsSchematic(User)) {
                GenericHash hash = GenericHash.Encode<SHA256>(User.Password, 8);

                User.Password = Convert.ToBase64String(hash.Salt) + Convert.ToBase64String(hash.Passhash);
                User.Salt = Convert.ToBase64String(hash.Salt);

                Users.InsertOne(User);
                return User;
            }
            return null;
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