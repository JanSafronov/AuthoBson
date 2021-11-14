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
        /// Returns optionally filtered list of all model bases
        /// </summary>
        /// <param name="filter">Model base filter</param>
        /// <param name="serializer">Model base serializer</param>
        /// <returns>Filtered list of model bases</returns>
        public List<I> GetAll<I>(FilterDefinition<M> filter = null, IBsonSerializer<I> serializer = null) where I : IModelBase =>
            (filter != null ? Items.Find(filter) : Items.Find(User => true))
            .As(serializer).ToList();

        /// <summary>
        /// Returns the serialized model base by and identificator property
        /// </summary>
        /// <typeparam name="I">Model to serialize into</typeparam>
        /// <param name="identificator">identificator property of such typed model base</param>
        /// <param name="serializer">Serializer of model base</param>
        /// <returns>Serialized model base</returns>
        public I Get<I>(KeyValuePair<string, string> identificator, IBsonSerializer<I> serializer = null) where I : IModelBase =>
            Items.Find(M => typeof(M).GetProperty(identificator.Key).GetValue(M) as string == identificator.Value).As(serializer).FirstOrDefault();

        public M Create(M M, Action<M> middleAction = null) =>
            Template.IsSchematic(M) ? ((Func<M>)(() => { middleAction(M); Items.InsertOne(M); return M; }))()
            : default;

        public M Replace(M M, KeyValuePair<string, string> identificator) =>
            Items.FindOneAndReplace(M => typeof(M).GetProperty(identificator.Key).GetValue(M) as string == identificator.Value, M);

        public M Update(KeyValuePair<string, string> identificator, UpdateDefinition<M> update) =>
            Items.FindOneAndUpdate(M => typeof(M).GetProperty(identificator.Key).GetValue(M) as string == identificator.Value, update);

        public M Remove(KeyValuePair<string, string> Identificator) =>
            Items.FindOneAndDelete(M => typeof(M).GetProperty(Identificator.Key).GetValue(M) as string == Identificator.Value);
    }
}
