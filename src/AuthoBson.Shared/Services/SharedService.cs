using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using AuthoBson.Shared.Data.Models;

namespace AuthoBson.Shared.Services {
    public abstract class SharedService<M> where M : ModelBase, IModelBase {
        IMongoCollection<M> Items { get; set; }

        IModelTemplate<M> Template { get; set; }

        public SharedService(IStoreDatabaseSettings settings, IModelTemplate<M> template) {
            MongoClient client = new(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            Items = database.GetCollection<M>(settings.CollectionName);

            Template = template;
        }

        public SharedService(IStoreDatabase settings, IModelTemplate<M> template) {
            MongoClient client = new();
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            Items = database.GetCollection<M>(settings.CollectionName);

            Template = template;
        }

        public List<I> GetAll<I>(FilterDefinition<M> filter = null, IBsonSerializer<I> serializer = null) where I : IModelBase =>
            (filter != null ? Items.Find(filter) : Items.Find(User => true))
            .As(serializer).ToList();

        public List<M> GetAll(FilterDefinition<M> filter = null) =>
            (filter != null ? Items.Find(filter) : Items.Find(User => true)).ToList();

        public I Get<I>(string Id, IBsonSerializer<I> serializer = null) where I : IModelBase =>
            Items.Find(M => M.Id == Id).As(serializer).FirstOrDefault();

        public M Get(string Id) =>
            Items.Find(M => M.Id == Id).FirstOrDefault();

        public M Create(M M, Action<M> middleAction = null) =>
            Template.IsSchematic(M) ? ((Func<M>)(() => { middleAction(M); Items.InsertOne(M); return M; }))() 
            : default;

        public bool Replace(M M, string Id) =>
            Items.ReplaceOne(M => M.Id == Id, M).IsAcknowledged;

        public M Remove(string Id) =>
            Items.FindOneAndDelete(M => M.Id == Id);
    }
}
