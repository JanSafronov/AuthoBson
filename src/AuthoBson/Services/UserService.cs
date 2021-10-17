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
using AuthoBson.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using AuthoBson.Models.Templates;
using AuthoBson.Services.Security;
using AuthoBson.Shared.Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AuthoBson.Services
{

    public class UserService {

        private IMongoCollection<IUser> Users { get; set; }

        private IUserTemplate Template { get; set; }
        
        public UserService(IUserstoreDatabaseSettings settings, IUserTemplate template) {
            MongoClient client = new(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            Users = database.GetCollection<IUser>(settings.UsersCollectionName);

            Template = template;
            
        }

        public UserService(IUserstoreDatabase settings, IUserTemplate template) {
            MongoClient client = new();
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            Users = database.GetCollection<IUser>(settings.UsersCollectionName);

            Template = template;
        }

        /// <summary>
        /// Returns the Users enumerable collection
        /// </summary>
        /// <returns>Users enumerable collection</returns>
        public List<User> GetAll () => Users.Find(User => true).As<User>(new SpecificBsonSerializer()).ToList();

        public List<IUser> GetAny (FilterDefinition<IUser> filter) => Users.Find(filter).ToList();

        /// <summary>
        /// Finds a User from an enumerable collection by username
        /// </summary>
        /// <param name="id">Id of the user to find</param>
        /// <returns>User object or null</returns>
        public IUser GetUser (string Id) => Users.Find(User => User.Id == Id).FirstOrDefault();

        public IUser CreateUser (IUser User) {
            if (Template.IsSchematic(User)) {
                GenericHash hash = GenericHash.Encode<SHA256>(User.Password, 8);

                User.Password = Convert.ToBase64String(hash.Salt) + Convert.ToBase64String(hash.Passhash);
                User.Salt = Convert.ToBase64String(hash.Salt);
                
                Users.InsertOne(User);
                return User;
            }
            return null;
        }

        public IUser ReplaceUser (string Id, IUser newUser) {
            Users.ReplaceOne(User => User.Id == Id, newUser);
            return newUser;
        }

        public IUser SuspendUser (string Id, Suspension Suspension) {
            UpdateDefinitionBuilder<IUser> bupdate = new();
            UpdateDefinition<IUser> update = bupdate.AddToSet("Suspension", Suspension);

            return Users.FindOneAndUpdate<IUser>(User => User.Id == Id, update);
        }

        public IUser RemoveUser (string Id) => Users.FindOneAndDelete(User => User.Id == Id);

        /// <summary>
        /// Morph a bson field by bsontype and by value as an argument of a function
        /// </summary>
        /// <param name="Id">Id of the user to find</param>
        /// <param name="key">Key of the field to change</param>
        /// <param name="functor">Endomorphic mapping between the type of the field</param>
        /// <typeparam name="B">BsonValue</typeparam>
        /// <returns>Morphed user</returns>
        public IUser ChangeField<B> (string Id, string key, Func<BsonValue, B> functor) where B : BsonValue {
            UserDocument doc = new(this.GetUser(Id));
            doc = doc.Functor<B>(key, functor);
            return BsonSerializer.Deserialize<User>(doc.User);
        }
    }

    public class SpecificBsonSerializer : IBsonSerializer<User> {
        public User Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) {
            BsonDocument bdoc = BsonSerializer.Deserialize<BsonDocument>(context.Reader);
            return new User() {
                Username = bdoc["Username"].AsString,
                Password = bdoc["Password"].AsString,
                Email = bdoc["Email"].AsString,
                Notification = bdoc["Notification"].AsBoolean,
                Verified = bdoc["Verified"].AsString,
                Joined = bdoc["Joined"].ToUniversalTime(),
                Role = (Role)bdoc["Role"].AsInt32,
                Suspension = new Suspension(bdoc["Suspension"].AsBsonDocument["Reason"].AsString, bdoc["Suspension"].AsBsonDocument["Duration"].ToUniversalTime())
            };
            //return new User(test, test, test, true, test, DateTime.Now, Role.Moderator, new Suspension(test, DateTime.Now));
            //return null;
        }
        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            //User user = BsonSerializer.Deserialize<User>(context.Reader);
            return null;
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, User obj) {
            context.Writer.WriteString(obj.Email);
        }
        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object obj) {
            if (obj is User user)
            {
                context.Writer.WriteString(user.Email);
            }
            else
            {
                throw new NotSupportedException("This is not an email");
            }
        }

        public Type ValueType => typeof(User);
    }
}