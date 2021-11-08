using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Security.Authentication;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using AuthoBson.Models;
using AuthoBson.Models.Templates;
using AuthoBson.Services.Security;
using AuthoBson.Shared.Data.Models;
using AuthoBson.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AuthoBson.Services
{

    public class UserService {

        private IMongoCollection<User> Users { get; set; }

        private UserTemplate Template { get; set; }
        
        public UserService(IStoreDatabaseSettings settings, UserTemplate template) {
            MongoClient client = new(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            Users = database.GetCollection<User>(settings.CollectionName);

            Template = template;
            
        }

        public UserService(IStoreDatabase settings, UserTemplate template) {
            MongoClient client = new();
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            Users = database.GetCollection<User>(settings.CollectionName);

            Template = template;
        }


        /// <summary>
        /// Returns a filtered list of all users
        /// </summary>
        /// <param name="filter">User's filter</param>
        /// <returns>Filtered list of users</returns>
        public List<User> GetAll (FilterDefinition<User> filter = null) => 
            (filter != null ? Users.Find(filter) : Users.Find(User => true))
            .As<User>(new SpecificBsonSerializer()).ToList();

        /// <summary>
        /// Find's the user by his Id
        /// </summary>
        /// <param name="id">Id of the user to find</param>
        /// <returns>Found user or null</returns>
        public User GetUser (string Id) => Users.Find(User => User.Id == Id).As<User>(new SpecificBsonSerializer()).FirstOrDefault();

        /// <summary>
        /// Creates a new user in the database's collection
        /// </summary>
        /// <param name="User">The user to insert in the database's collection</param>
        /// <returns>The inserted user</returns>
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

        /// <summary>
        /// Replaces a uses identified by his Id with a new one
        /// </summary>
        /// <param name="Id">Identification of the User to replace</param>
        /// <param name="newUser">The new user to replace with</param>
        /// <returns>Replaced user</returns>
        public User ReplaceUser (string Id, User newUser) {
            Users.ReplaceOne(User => User.Id == Id, newUser);
            return newUser;
        }

        /// <summary>
        /// Suspends a user identified by his Id with a Suspension update
        /// </summary>
        /// <param name="Id">Id of the user to suspend</param>
        /// <param name="Suspension">Suspension update for the user</param>
        /// <returns>Suspended user</returns>
        public User SuspendUser (string Id, Suspension Suspension) {
            UpdateDefinitionBuilder<User> bupdate = new();
            UpdateDefinition<User> update = bupdate.AddToSet("Suspension", Suspension);

            return Users.FindOneAndUpdate<User>(User => User.Id == Id, update);
        }

        /// <summary>
        /// Removes a user identified by his Id
        /// </summary>
        /// <param name="Id">Id of the user to suspend</param>
        /// <returns>Removed user</returns>
        public User RemoveUser (string Id) => Users.FindOneAndDelete(User => User.Id == Id);

        /// <summary>
        /// Morph a bson field by bsontype and by value as an argument of a function
        /// </summary>
        /// <param name="Id">Id of the user</param>
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