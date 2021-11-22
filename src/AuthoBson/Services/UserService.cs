﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using AuthoBson.Models;
using AuthoBson.Shared;
using AuthoBson.Shared.Data.Models;
using AuthoBson.Shared.Services;
using AuthoBson.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using AuthoBson.Shared.Extensions;

namespace AuthoBson.Services
{

    public class UserService : SharedService<User> {

        private IMongoCollection<User> Users { get; set; }

        private UserTemplate Template { get; set; }

        private SecurityMechanism<User, SHA256> Mechanism { get => new(); set => Mechanism = value; }

        public UserService(IStoreDatabaseSettings settings, UserTemplate template) :
            base(settings, template)
        { }

        /// <summary>
        /// Returns optionally filtered list of all users
        /// </summary>
        /// <param name="filter">Users filter</param>
        /// <returns>Filtered list of users</returns>
        public List<User> GetAll(FilterDefinition<User> filter = null) =>
            GetAll(filter, UserBsonSerializer.Instance);

        /// <summary>
        /// Find a user by his Id
        /// </summary>
        /// <param name="id">Id of the user to find</param>
        /// <returns>Found user or null</returns>
        public User GetUser(string id) =>
            Get(id, UserBsonSerializer.Instance);

        /// <summary>
        /// Asynchronously find a user by his Id
        /// </summary>
        /// <param name="id">Id of the user to find</param>
        /// <returns>Found user or null</returns>
        public async Task<User> GetUserAsync(string id) =>
            await GetAsync(id, UserBsonSerializer.Instance);

        /// <summary>
        /// Login the user by his username and password
        /// </summary>
        /// <param name="username">Username of the user to find</param>
        /// <param name="password">Password of the user to find</param>
        /// <returns>Found user or null</returns>
        public User LoginUser([Unique("Username")] string username, string password)
        {
            UpdateDefinitionBuilder<User> bupdate = new();
            UpdateDefinition<User> update = bupdate.AddToSet("Active", true);
            
            return Users.FindOneAndUpdate(user => user.Username == username && Mechanism.VerifyCredential(user, password, "Password", "Salt"), update);
        }

        /// <summary>
        /// Asynchronously login the user by his username and password
        /// </summary>
        /// <param name="username">Username of the user to find</param>
        /// <param name="password">Password of the user to find</param>
        /// <returns>Found user or null</returns>
        public async Task<User> LoginUserAsync([Unique("Username")] string username, string password)
        {
            UpdateDefinitionBuilder<User> bupdate = new();
            UpdateDefinition<User> update = await Task.FromResult(bupdate.AddToSet("Active", true));

            return UpdateAsync(KeyValuePair.Create("Username", username), update, user => Mechanism.VerifyCredential(user, password, "Password", "Salt")).Result;

            //await Users.FindOneAndUpdateAsync(user => user.Username == username && Mechanism.VerifyCredential(user, password, "Password", "Salt"), update);
        }

        /// <summary>
        /// Creates a new user in the database's collection
        /// </summary>
        /// <param name="user">The user to insert in the database's collection</param>
        /// <returns>The inserted user</returns>
        public User CreateUser(User user) =>
            Create(user, user => Mechanism.HashCredential(user, "Password", "Salt"));

        /// <summary>
        /// Asynchronously creates a new user in the database's collection
        /// </summary>
        /// <param name="user">The user to insert in the database's collection</param>
        /// <returns>The inserted user</returns>
        public async Task<User> CreateUserAsync(User user) =>
            await CreateAsync(user, user => Mechanism.HashCredential(user, "Password", "Salt"));

        /// <summary>
        /// Replaces a uses identified by his Id with a new one
        /// </summary>
        /// <param name="id">Id of the user to replace</param>
        /// <param name="newUser">The new user to replace with</param>
        /// <returns>Whether the user was replaced</returns>
        public User ReplaceUser(User newUser, string id) =>
            Replace(newUser, id);

        /// <summary>
        /// Asynchronously replaces a uses identified by his Id with a new one
        /// </summary>
        /// <param name="id">Id of the user to replace</param>
        /// <param name="newUser">The new user to replace with</param>
        /// <returns>Whether the user was replaced</returns>
        public async Task<User> ReplaceUserAsync(User newUser, string id) =>
            await ReplaceAsync(newUser, id);

        /// <summary>
        /// Update the user by his id with property-value pairs
        /// </summary>
        /// <param name="id">Username of the user to find and update</param>
        /// <param name="pairs">Pairs of key-values to update in the user</param>
        /// <returns>Found & updated user or null</returns>
        public User UpdateUser(IDictionary<string, object> pairs, string id) =>
            Update(id, new UpdateDefinitionBuilder<User>().PushMultiple(pairs));

        /// <summary>
        /// Update the user by his username with property-value pair
        /// </summary>
        /// <param name="id">Id of the user to find and update</param>
        /// <param name="pair">Pairs of key-values to update in the user</param>
        /// <returns>Found & updated user or null</returns>
        public User UpdateUser(KeyValuePair<string, object> pair, string id) =>
            Update(id, new UpdateDefinitionBuilder<User>().AddToSet(pair.Key, pair.Value));

        /// <summary>
        /// Asynchronously update the user by his id with property-value pairs
        /// </summary>
        /// <param name="id">Username of the user to find and update</param>
        /// <param name="pairs">Pairs of key-values to update in the user</param>
        /// <returns>Found & updated user or null</returns>
        public async Task<User> UpdateUserAsync(IDictionary<string, object> pairs, string id) =>
            await UpdateAsync(id, new UpdateDefinitionBuilder<User>().PushMultiple(pairs));

        /// <summary>
        /// Asynchronously update the user by his username with property-value pair
        /// </summary>
        /// <param name="id">Id of the user to find and update</param>
        /// <param name="pair">Pairs of key-values to update in the user</param>
        /// <returns>Found & updated user or null</returns>
        public async Task<User> UpdateUserAsync(KeyValuePair<string, object> pair, string id) =>
            await UpdateAsync(id, new UpdateDefinitionBuilder<User>().AddToSet(pair.Key, pair.Value));

        /// <summary>
        /// Suspends a user identified by his id with a suspension update
        /// </summary>
        /// <param name="id">Id of the user to suspend</param>
        /// <param name="suspension">Suspension update for the user</param>
        /// <returns>Suspended user</returns>
        public User SuspendUser(Suspension suspension, string id) =>
            Update(id, new UpdateDefinitionBuilder<User>().AddToSet("Suspension", suspension));

        /// <summary>
        /// Asynchronously suspends a user identified by his id with a suspension update
        /// </summary>
        /// <param name="id">Id of the user to suspend</param>
        /// <param name="suspension">Suspension update for the user</param>
        /// <returns>Suspended user</returns>
        public async Task<User> SuspendUserAsync(Suspension suspension, string id) =>
            await UpdateAsync(id, new UpdateDefinitionBuilder<User>().AddToSet("Suspension", suspension));

        /// <summary>
        /// Removes a user identified by his Id
        /// </summary>
        /// <param name="id">Id of the user to suspend</param>
        /// <returns>Removed user</returns>
        public User RemoveUser(string id) =>
            Remove(id);

        /// <summary>
        /// Asynchronously removes a user identified by his Id
        /// </summary>
        /// <param name="id">Id of the user to suspend</param>
        /// <returns>Removed user</returns>
        public async Task<User> RemoveUserAsync(string id) =>
            await RemoveAsync(id);

        /// <summary>
        /// Morph a bson field by bsontype and by value as an argument of a function
        /// </summary>
        /// <typeparam name="B">BsonValue</typeparam>
        /// <param name="id">Id of the user</param>
        /// <param name="key">Key of the field to change</param>
        /// <param name="functor">Endomorphic mapping between the type of the field</param>
        /// <returns>Morphed user</returns>
        public User ChangeField<B>(string id, string key, Func<BsonValue, B> functor) where B : BsonValue {
            UserDocument doc = new(this.GetUser(id));
            doc = doc.Functor(key, functor);
            return BsonSerializer.Deserialize<User>(doc.User);
        }
    }
}
