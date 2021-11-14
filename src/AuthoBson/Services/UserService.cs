using System;
using System.Collections;
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
using AuthoBson.Shared;
using AuthoBson.Shared.Data.Models;
using AuthoBson.Shared.Services;
using AuthoBson.Shared.Services.Security;
using AuthoBson.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AuthoBson.Services
{

    public class UserService : SharedService<User> {

        private IMongoCollection<User> Users { get; set; }

        private UserTemplate Template { get; set; }

        private SecurityMechanism<User, SHA256> Mechanism { get; }
        
        public UserService(IStoreDatabaseSettings settings, UserTemplate template) :
            base(settings, template)
        { }

        public UserService(IStoreDatabase settings, UserTemplate template) :
            base(settings, template)
        { }

        /// <summary>
        /// Returns optionally filtered list of all users
        /// </summary>
        /// <param name="filter">User filter</param>
        /// <returns>Filtered list of users</returns>
        public List<User> GetAll(FilterDefinition<User> filter = null) =>
            base.GetAll<User>(filter, UserBsonSerializer.Instance);

        /// <summary>
        /// Find the user by his Id
        /// </summary>
        /// <param name="id">Id of the user to find</param>
        /// <returns>Found user or null</returns>
        public User GetUser([Unique("Id")] string Id) =>
            base.Get(Id, UserBsonSerializer.Instance);

        /// <summary>
        /// Login the user by his username and password
        /// </summary>
        /// <param name="Username">Username of the user to find</param>
        /// <param name="Password">Password of the user to find</param>
        /// <returns>Found user or null</returns>
        public User LoginUser([Unique("Username")] string Username, string Password)
        {
            UpdateDefinitionBuilder<User> bupdate = new();
            UpdateDefinition<User> update = bupdate.AddToSet("Active", true);

            return Users.FindOneAndUpdate(M => M.Username == Username && Mechanism.VerifyCredential(M, Password, "Password", "Salt"), update);
        }

        /// <summary>
        /// Creates a new user in the database's collection
        /// </summary>
        /// <param name="User">The user to insert in the database's collection</param>
        /// <returns>The inserted user</returns>
        public User CreateUser(User User) =>
            base.Create(User, User => Mechanism.HashCredential(User, "Password", "Salt"));

        /// <summary>
        /// Replaces a uses identified by his Id with a new one
        /// </summary>
        /// <param name="Id">Identification of the User to replace</param>
        /// <param name="newUser">The new user to replace with</param>
        /// <returns>Whether the user was replaced</returns>
        public User ReplaceUser(User newUser, string Id) =>
            base.Replace(newUser, Id);

        /// <summary>
        /// Update the user by his username with property-value pairs
        /// </summary>
        /// <param name="Username">Username of the user to find and update</param>
        /// <param name="Pairs">Pairs of key-values to update in the user</param>
        /// <returns>Found & updated user or null</returns>
        public User UpdateUser([Unique("Username")] string Username, IDictionary<string, object> Pairs)
        {
            UpdateDefinition<User> update = new BsonDocumentUpdateDefinition<User>(Pairs.ToBsonDocument());

            return Users.FindOneAndUpdate(M => M.Username == Username, update);
        }

        /// <summary>
        /// Update the user by his username with property-value pairs
        /// </summary>
        /// <param name="Username">Username of the user to find and update</param>
        /// <param name="Pairs">Pairs of key-values to update in the user</param>
        /// <returns>Found & updated user or null</returns>
        public User UpdateUser([Unique("Username")] string Username, KeyValuePair<string, object> Pair)
        {
            UpdateDefinitionBuilder<User> bupdate = new();
            UpdateDefinition<User> update = bupdate.AddToSet(Pair.Key, Pair.Value);

            return Users.FindOneAndUpdate(M => M.Username == Username, update);
        }

        /// <summary>
        /// Suspends a user identified by his Id with a Suspension update
        /// </summary>
        /// <param name="Id">Id of the user to suspend</param>
        /// <param name="Suspension">Suspension update for the user</param>
        /// <returns>Suspended user</returns>
        public User SuspendUser(Suspension Suspension, string Id) {
            UpdateDefinitionBuilder<User> bupdate = new();
            UpdateDefinition<User> update = bupdate.AddToSet("Suspension", Suspension);

            return Users.FindOneAndUpdate(User => User.Id == Id, update);
        }

        /// <summary>
        /// Removes a user identified by his Id
        /// </summary>
        /// <param name="Id">Id of the user to suspend</param>
        /// <returns>Removed user</returns>
        public User RemoveUser(string Id) =>
            base.Remove(Id);

        /// <summary>
        /// Morph a bson field by bsontype and by value as an argument of a function
        /// </summary>
        /// <param name="Id">Id of the user</param>
        /// <param name="key">Key of the field to change</param>
        /// <param name="functor">Endomorphic mapping between the type of the field</param>
        /// <typeparam name="B">BsonValue</typeparam>
        /// <returns>Morphed user</returns>
        public User ChangeField<B>(string Id, string key, Func<BsonValue, B> functor) where B : BsonValue {
            UserDocument doc = new(this.GetUser(Id));
            doc = doc.Functor<B>(key, functor);
            return BsonSerializer.Deserialize<User>(doc.User);
        }
    }
}
