using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using AuthoBson.Shared;
using AuthoBson.Shared.Data.Models;
using AuthoBson.Shared.Services.Security;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace AuthoBson.Shared.Services
{
    public abstract class SharedService<M> where M : ModelBase
    {
        IMongoCollection<M> Items { get; set; }

        IModelTemplate<M> Template { get; set; }

        SecurityMechanism<M, SHA256> Mechanism { get; set; }

        public SharedService(IStoreDatabaseSettings settings, IModelTemplate<M> template)
        {
            MongoClient client = new(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            Items = database.GetCollection<M>(settings.CollectionName);

            Template = template;
            Mechanism = new SecurityMechanism<M, SHA256>();
        }

        public SharedService(IStoreDatabase settings, IModelTemplate<M> template)
        {
            MongoClient client = new();
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            Items = database.GetCollection<M>(settings.CollectionName);

            Template = template;
            Mechanism = new SecurityMechanism<M, SHA256>();
        }

        /// <summary>
        /// Returns optionally filtered list of all serialized model bases
        /// </summary>
        /// <param name="filter">Model base filter</param>
        /// <param name="serializer">Model base serializer</param>
        /// <returns>Filtered list of model bases</returns>
        public List<I> GetAll<I>(FilterDefinition<M> filter = null, IBsonSerializer<I> serializer = null) where I : IModelBase =>
            (filter != null ? Items.Find(filter) : Items.Find(User => true))
            .As(serializer).ToList();

        //public static readonly Predicate<M> AlwaysTrue = ignored => true;

        /// <summary>
        /// Returns the serialized model base by id and optional condition
        /// </summary>
        /// <typeparam name="I">Model to serialize into</typeparam>
        /// <param name="id">Id of the user to get</param>
        /// <param name="serializer">Serializer of model base</param>
        /// <param name="condition">Optional condition for finding the model base</param>
        /// <returns>Serialized model base</returns>
        public I Get<I>(string id, IBsonSerializer<I> serializer = null, Predicate<M> condition = null) where I : IModelBase =>
            Items.Find(M => M.Id == id && (condition ?? (ignored => true))(M)).As(serializer).FirstOrDefault();

        /// <summary>
        /// Returns the serialized model base by identificator property and optional condition
        /// </summary>
        /// <typeparam name="I">Model to serialize into</typeparam>
        /// <param name="identificator">Identificator property of such typed model base</param>
        /// <param name="serializer">Serializer of model base</param>
        /// <param name="condition">Optional condition for finding the model base</param>
        /// <returns>Serialized model base or null else</returns>
        public I Get<I>(KeyValuePair<string, string> identificator, IBsonSerializer<I> serializer = null, Predicate<M> condition = null) where I : IModelBase =>
            Items.Find(M => typeof(M).GetProperty(identificator.Key).GetValue(M) as string == identificator.Value && (condition ?? (ignored => true))(M)).As(serializer).FirstOrDefault();

        /// <summary>
        /// Creates a new base model with optional action to handle it
        /// </summary>
        /// <param name="M">Model base to insert</param>
        /// <param name="middleAction">Action to handle the model before insertion</param>
        /// <returns>Created base model or null else</returns>
        public M Create(M M, Action<M> middleAction = null) =>
            Template.IsSchematic(M) ? ((Func<M>)(() => { middleAction(M); Items.InsertOne(M); return M; }))()
            : null;

        /// <summary>
        /// Replace a model base by id and optional condition with a new one
        /// </summary>
        /// <param name="M">Model base for replacement</param>
        /// <param name="id">Id of the user to replace</param>
        /// <param name="condition">Optional condition for finding the model base</param>
        /// <returns>Replaced model base or null else</returns>
        public M Replace(M M, string id, Predicate<M> condition = null) =>
            Items.FindOneAndReplace(M => M.Id == id && (condition ?? (ignored => true))(M), M);

        /// <summary>
        /// Replace a model base by identificator property and optional condition with a new one
        /// </summary>
        /// <param name="M">Model base for replacement</param>
        /// <param name="identificator">Identificator property of such typed model base</param>
        /// <param name="condition">Optional condition for finding the model base</param>
        /// <returns>Replaced model base or null else</returns>
        public M Replace(M M, KeyValuePair<string, string> identificator, Predicate<M> condition = null) =>
            Items.FindOneAndReplace(M => typeof(M).GetProperty(identificator.Key).GetValue(M) as string == identificator.Value && (condition ?? (ignored => true))(M), M);

        /// <summary>
        /// Update a model base definitively by id and optional condition
        /// </summary>
        /// <param name="id">Id of the user to update</param>
        /// <param name="update">Update definition for the model base</param>
        /// <param name="condition">Optional condition for finding the model base</param>
        /// <returns>Updated model base or null else</returns>
        public M Update(string id, UpdateDefinition<M> update, Predicate<M> condition = null) =>
            Items.FindOneAndUpdate(M => M.Id == id && (condition ?? (ignored => true))(M), update);

        /// <summary>
        /// Update a model base definitively by identificator property and optional condition
        /// </summary>
        /// <param name="identificator">Identificator property of such typed model base</param>
        /// <param name="update">Update definition for the model base</param>
        /// <param name="condition">Optional condition for finding the model base</param>
        /// <returns>Updated model base or null else</returns>
        public M Update(KeyValuePair<string, string> identificator, UpdateDefinition<M> update, Predicate<M> condition = null) =>
            Items.FindOneAndUpdate(M => typeof(M).GetProperty(identificator.Key).GetValue(M) as string == identificator.Value && (condition ?? (ignored => true))(M), update);

        /// <summary>
        /// Remove a model base by id and optional condition
        /// </summary>
        /// <param name="id">Id of the user to update</param>
        /// <param name="condition">Optional condition for finding the model base</param>
        /// <returns>Removed model base or null else</returns>
        public M Remove(string id, Predicate<M> condition = null) =>
            Items.FindOneAndDelete(M => M.Id == id && (condition ?? (ignored => true))(M));

        /// <summary>
        /// Remove a model base by identificator property and optional condition
        /// </summary>
        /// <param name="Identificator">Identificator property of such typed model base</param>
        /// <param name="condition">Optional condition for finding the model base</param>
        /// <returns>Removed model base or null else</returns>
        public M Remove(KeyValuePair<string, string> Identificator, Predicate<M> condition = null) =>
            Items.FindOneAndDelete(M => typeof(M).GetProperty(Identificator.Key).GetValue(M) as string == Identificator.Value && (condition ?? (ignored => true))(M));
    }
}
