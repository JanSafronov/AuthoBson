using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using AuthoBson.Shared;
using AuthoBson.Shared.Data.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace AuthoBson.Shared.Services
{
    public interface ISharedService
    {
        
    }

    public abstract class SharedService<M> : ISharedService where M : ModelBase
    {
        protected IMongoCollection<M> Items { get; set; }

        internal IModelTemplate<M> Template { get; set; }

        protected SecurityMechanism<M, SHA256> Mechanism = new();
        
        public SharedService(IStoreDatabaseSettings settings, IModelTemplate<M> template)
        {
            MongoClient client = new(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            Items = database.GetCollection<M>(settings.CollectionName);

            Template = template;
            Mechanism = new();
        }

        /// <summary>
        /// Returns optionally filtered list of all serialized model bases
        /// </summary>
        /// <param name="filter">Model base filter</param>
        /// <param name="serializer">Model base serializer</param>
        /// <returns>Filtered list of model bases</returns>
        protected List<I> GetAll<I>(Expression<Func<M, bool>> filter, IBsonSerializer<I> serializer = null) where I : IModelBase =>
            (filter != null ? Items.Find(filter) : Items.Find(User => true))
            .As(serializer).ToList();

        /// <summary>
        /// Returns the serialized model base by id
        /// </summary>
        /// <typeparam name="I">Model to serialize into</typeparam>
        /// <param name="id">Id of the model base to get</param>
        /// <param name="serializer">Serializer of model base</param>
        /// <returns>Serialized model base</returns>
        protected I Get<I>(string id, IBsonSerializer<I> serializer = null) where I : IModelBase =>
            Items.Find(M => M.Id == id).As(serializer).FirstOrDefault();

        /// <summary>
        /// Returns the serialized model base by id and another condition
        /// </summary>
        /// <typeparam name="I">Model to serialize into</typeparam>
        /// <param name="id">Id of the model base to get</param>
        /// <param name="condition">Optional condition for finding the model base</param>
        /// <param name="serializer">Serializer of model base</param>
        /// <returns>Serialized model base</returns>
        protected I Get<I>(string id, Predicate<M> condition, IBsonSerializer<I> serializer = null) where I : IModelBase =>
            Items.Find(M => M.Id == id && condition(M)).As(serializer).FirstOrDefault();

        /// <summary>
        /// Returns the serialized model base by identificator property and optional condition
        /// </summary>
        /// <typeparam name="I">Model to serialize into</typeparam>
        /// <param name="identificator">Identificator property of such typed model base</param>
        /// <param name="serializer">Serializer of model base</param>
        /// <returns>Serialized model base or null else</returns>
        protected I Get<I>(KeyValuePair<string, string> identificator, IBsonSerializer<I> serializer = null) where I : IModelBase =>
            Items.Find(M => typeof(M).GetProperty(identificator.Key).GetValue(M) as string == identificator.Value).As(serializer).FirstOrDefault();

        /// <summary>
        /// Returns the serialized model base by identificator property and optional condition
        /// </summary>
        /// <typeparam name="I">Model to serialize into</typeparam>
        /// <param name="identificator">Identificator property of such typed model base</param>
        /// <param name="serializer">Serializer of model base</param>
        /// <param name="condition">Optional condition for finding the model base</param>
        /// <returns>Serialized model base or null else</returns>
        protected I Get<I>(KeyValuePair<string, string> identificator, Predicate<M> condition, IBsonSerializer<I> serializer = null) where I : IModelBase =>
            Items.Find(M => typeof(M).GetProperty(identificator.Key).GetValue(M) as string == identificator.Value && condition(M)).As(serializer).FirstOrDefault();

        /// <summary>
        /// Asynchronously returns the serialized model base by id
        /// </summary>
        /// <typeparam name="I">Model to serialize into</typeparam>
        /// <param name="id">Id of the model base to get</param>
        /// <param name="serializer">Serializer of model base</param>
        /// <returns>Serialized model base</returns>
        protected async Task<I> GetAsync<I>(string id, IBsonSerializer<I> serializer = null) where I : IModelBase =>
            await Task.FromResult(Items.Find(M => M.Id == id).As(serializer).FirstOrDefault());

        /// <summary>
        /// Asynchronously returns the serialized model base by id and another condition
        /// </summary>
        /// <typeparam name="I">Model to serialize into</typeparam>
        /// <param name="id">Id of the model base to get</param>
        /// <param name="condition">Optional condition for finding the model base</param>
        /// <param name="serializer">Serializer of model base</param>
        /// <returns>Serialized model base</returns>
        protected async Task<I> GetAsync<I>(string id, Predicate<M> condition, IBsonSerializer<I> serializer = null) where I : IModelBase =>
            await Task.FromResult(Items.Find(M => M.Id == id && condition(M)).As(serializer).FirstOrDefault());

        /// <summary>
        /// Asynchronously returns the serialized model base by identificator property and optional condition
        /// </summary>
        /// <typeparam name="I">Model to serialize into</typeparam>
        /// <param name="identificator">Identificator property of such typed model base</param>
        /// <param name="serializer">Serializer of model base</param>
        /// <returns>Serialized model base or null else</returns>
        protected async Task<I> GetAsync<I>(KeyValuePair<string, string> identificator, IBsonSerializer<I> serializer = null) where I : IModelBase =>
            await Task.FromResult(Items.Find(M => typeof(M).GetProperty(identificator.Key).GetValue(M) as string == identificator.Value).As(serializer).FirstOrDefault());

        /// <summary>
        /// Asynchronously returns the serialized model base by identificator property and optional condition
        /// </summary>
        /// <typeparam name="I">Model to serialize into</typeparam>
        /// <param name="identificator">Identificator property of such typed model base</param>
        /// <param name="serializer">Serializer of model base</param>
        /// <param name="condition">Optional condition for finding the model base</param>
        /// <returns>Serialized model base or null else</returns>
        protected async Task<I> GetAsync<I>(KeyValuePair<string, string> identificator, Predicate<M> condition, IBsonSerializer<I> serializer = null) where I : IModelBase =>
            await Task.FromResult(Items.Find(M => typeof(M).GetProperty(identificator.Key).GetValue(M) as string == identificator.Value && condition(M)).As(serializer).FirstOrDefault());

        /// <summary>
        /// Creates a new base model
        /// </summary>
        /// <param name="M">Model base to insert</param>
        /// <returns>Created base model or null else</returns>
        protected M Create(M M) =>
            Template.IsSchematic(M) ? ((Func<M>)(() => { Items.InsertOne(M); return M; }))()
            : null;

        /// <summary>
        /// Creates a new base model with an action to handle it
        /// </summary>
        /// <param name="M">Model base to insert</param>
        /// <param name="middleAction">Action to handle the model before insertion</param>
        /// <returns>Created base model or null else</returns>
        protected M Create(M M, Action<M> middleAction) =>
            Template.IsSchematic(M) ? ((Func<M>)(() => { middleAction(M); Items.InsertOne(M); return M; }))()
            : null;

        /// <summary>
        /// Asynchronously creates a new base model
        /// </summary>
        /// <param name="M">Model base to insert</param>
        /// <returns>Created base model or null else</returns>
        protected async Task<M> CreateAsync(M M) =>
            await (Template.IsSchematic(M) ? Task.FromResult(((Func<M>)(() => { Items.InsertOne(M); return M; }))())
            : null);

        /// <summary>
        /// Asynchronously creates a new base model with an action to handle it
        /// </summary>
        /// <param name="M">Model base to insert</param>
        /// <param name="middleAction">Action to handle the model before insertion</param>
        /// <returns>Created base model or null else</returns>
        protected async Task<M> CreateAsync(M M, Action<M> middleAction) =>
            await (Template.IsSchematic(M) ? Task.FromResult(((Func<M>)(() => { middleAction(M); Items.InsertOneAsync(M); return M; }))())
            : null);

        /// <summary>
        /// Replace a model base by id and optional condition with a new one
        /// </summary>
        /// <param name="M">Model base for replacement</param>
        /// <param name="id">Id of the model base to replace</param>
        /// <returns>Replaced model base or null else</returns>
        protected M Replace(M M, string id) =>
            Items.FindOneAndReplace(M => M.Id == id, M);

        /// <summary>
        /// Replace a model base by id and optional condition with a new one
        /// </summary>
        /// <param name="M">Model base for replacement</param>
        /// <param name="id">Id of the model base to replace</param>
        /// <param name="condition">Optional condition for finding the model base</param>
        /// <returns>Replaced model base or null else</returns>
        protected M Replace(M M, string id, Predicate<M> condition = null) =>
            Items.FindOneAndReplace(M => M.Id == id && (condition ?? (ignored => true))(M), M);

        /// <summary>
        /// Replace a model base by optional condition with a new one
        /// </summary>
        /// <param name="M">Model base for replacement</param>
        /// <param name="condition">Optional condition for finding the model base</param>
        /// <returns>Replaced model base or null else</returns>
        protected M Replace(M M, Predicate<M> condition = null) =>
            Items.FindOneAndReplace(M => condition(M), M);

        /// <summary>
        /// Asynchronously replace a model base by id and optional condition with a new one
        /// </summary>
        /// <param name="M">Model base for replacement</param>
        /// <param name="id">Id of the model base to replace</param>
        /// <returns>Replaced model base or null else</returns>
        protected async Task<M> ReplaceAsync(M M, string id) =>
            await Items.FindOneAndReplaceAsync(M => M.Id == id, M);

        /// <summary>
        /// Asynchronously replace a model base by id and optional condition with a new one
        /// </summary>
        /// <param name="M">Model base for replacement</param>
        /// <param name="id">Id of the model base to replace</param>
        /// <param name="condition">Optional condition for finding the model base</param>
        /// <returns>Replaced model base or null else</returns>
        protected async Task<M> ReplaceAsync(M M, string id, Predicate<M> condition = null) =>
            await Items.FindOneAndReplaceAsync(M => M.Id == id && (condition ?? (ignored => true))(M), M);

        /// <summary>
        /// Asynchronously replace a model base by optional condition with a new one
        /// </summary>
        /// <param name="M">Model base for replacement</param>
        /// <param name="condition">Optional condition for finding the model base</param>
        /// <returns>Replaced model base or null else</returns>
        protected async Task<M> ReplaceAsync(M M, Predicate<M> condition = null) =>
            await Items.FindOneAndReplaceAsync(M => condition(M), M);

        /// <summary>
        /// Update a model base definitively by id and optional condition
        /// </summary>
        /// <param name="id">Id of the model base to update</param>
        /// <param name="update">Update definition for the model base</param>
        /// <returns>Updated model base or null else</returns>
        protected M Update(string id, UpdateDefinition<M> update) =>
            Items.FindOneAndUpdate(M => M.Id == id, update);

        /// <summary>
        /// Update a model base definitively by id and optional condition
        /// </summary>
        /// <param name="id">Id of the model base to update</param>
        /// <param name="update">Update definition for the model base</param>
        /// <param name="condition">Optional condition for finding the model base</param>
        /// <returns>Updated model base or null else</returns>
        protected M Update(string id, UpdateDefinition<M> update, Predicate<M> condition = null) =>
            Items.FindOneAndUpdate(M => M.Id == id && condition(M), update);

        /// <summary>
        /// Update a model base definitively by optional condition
        /// </summary>
        /// <param name="update">Update definition for the model base</param>
        /// <param name="condition">Optional condition for finding the model base</param>
        /// <returns>Updated model base or null else</returns>
        protected M Update(UpdateDefinition<M> update, Expression<Func<M, bool>> condition) =>
            Items.FindOneAndUpdate(condition, update);

        /// <summary>
        /// Asynchronously update a model base definitively by id and optional condition
        /// </summary>
        /// <param name="id">Id of the model base to update</param>
        /// <param name="update">Update definition for the model base</param>
        /// <returns>Updated model base or null else</returns>
        protected async Task<M> UpdateAsync(string id, UpdateDefinition<M> update) =>
            await Items.FindOneAndUpdateAsync(M => M.Id == id, update);

        /// <summary>
        /// Asynchronously update a model base definitively by id and optional condition
        /// </summary>
        /// <param name="id">Id of the model base to update</param>
        /// <param name="update">Update definition for the model base</param>
        /// <param name="condition">Optional condition for finding the model base</param>
        /// <returns>Updated model base or null else</returns>
        protected async Task<M> UpdateAsync(string id, UpdateDefinition<M> update, Predicate<M> condition = null) =>
            await Items.FindOneAndUpdateAsync(M => M.Id == id && condition(M), update);

        /// <summary>
        /// Asynchronously update a model base definitively by optional condition
        /// </summary>
        /// <param name="update">Update definition for the model base</param>
        /// <param name="condition">Optional condition for finding the model base</param>
        /// <returns>Updated model base or null else</returns>
        protected async Task<M> UpdateAsync(UpdateDefinition<M> update, Predicate<M> condition = null) =>
            await Items.FindOneAndUpdateAsync(M => condition(M), update);

        /// <summary>
        /// Remove a model base by id and optional condition
        /// </summary>
        /// <param name="id">Id of the model base to update</param>
        /// <returns>Removed model base or null else</returns>
        protected M Remove(string id) =>
            Items.FindOneAndDelete(M => M.Id == id);

        /// <summary>
        /// Remove a model base by id and optional condition
        /// </summary>
        /// <param name="id">Id of the model base to update</param>
        /// <param name="condition">Optional condition for finding the model base</param>
        /// <returns>Removed model base or null else</returns>
        protected M Remove(string id, Predicate<M> condition = null) =>
            Items.FindOneAndDelete(M => M.Id == id && condition(M));

        /// <summary>
        /// Remove a model base by optional condition
        /// </summary>
        /// <param name="condition">Optional condition for finding the model base</param>
        /// <returns>Removed model base or null else</returns>
        protected M Remove(Predicate<M> condition = null) =>
            Items.FindOneAndDelete(M => condition(M));

        /// <summary>
        /// Asynchronously remove a model base by id and optional condition
        /// </summary>
        /// <param name="id">Id of the model base to update</param>
        /// <returns>Removed model base or null else</returns>
        protected async Task<M> RemoveAsync(string id) =>
            await Items.FindOneAndDeleteAsync(M => M.Id == id);

        /// <summary>
        /// Asynchronously remove a model base by id and optional condition
        /// </summary>
        /// <param name="id">Id of the model base to update</param>
        /// <param name="condition">Optional condition for finding the model base</param>
        /// <returns>Removed model base or null else</returns>
        protected async Task<M> RemoveAsync(string id, Predicate<M> condition = null) =>
            await Items.FindOneAndDeleteAsync(M => M.Id == id && condition(M));

        /// <summary>
        /// Asynchronously remove a model base by optional condition
        /// </summary>
        /// <param name="condition">Optional condition for finding the model base</param>
        /// <returns>Removed model base or null else</returns>
        protected async Task<M> RemoveAsync(Predicate<M> condition = null) =>
            await Items.FindOneAndDeleteAsync(M => condition(M));
    }

    public abstract class SharedRoutedService<M, R> : SharedService<M>, ISharedService where M : ModelBase where R : ModelBase
    {
        protected IMongoCollection<R>[] Routes { get; set; }

        public SharedRoutedService(IStoreDatabaseSettings settings, IModelTemplate<M> template) :
            base(settings, template)
        { }

        public SharedRoutedService(IRoutedDatabaseSettings settings, IModelTemplate<M> template) :
            base(settings, template)
        {
            MongoClient client = new(settings.ConnectionString);

            Array.ForEach(settings.Routes, r => Array.ForEach(r.Value, v => client.GetDatabase(r.Key).GetCollection<R>(v)));
        }

        /// <summary>
        /// Check whether the document retrieved by id from the indexed route exists there
        /// </summary>
        /// <typeparam name="I">Model to serialize into</typeparam>
        /// <param name="index">Index of the routed collection</param>
        /// <param name="id">Id of the model base to update</param>
        /// <param name="serializer">Serializer of model base</param>
        /// <returns>Whether the specified model base exists in the route</returns>
        protected bool ExistsInRoute<I>(int index, string id, IBsonSerializer<I> serializer) where I : IModelBase =>
            Routes[index].Find(R => R.Id == id).As(serializer).FirstOrDefault() != null;
    }
}
