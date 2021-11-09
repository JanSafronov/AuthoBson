using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using AuthoBson.Shared.Data.Models;

namespace AuthoBson.Shared.Services {
    public abstract class SharedService<M> where M : IModelBase {
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

        public List<I> GetAll<I>(FilterDefinition<M> filter = null, IBsonSerializer<I> serializer = null) where I : ModelBase =>
            (filter != null ? Items.Find(filter) : Items.Find(User => true))
            .As(serializer).ToList();

        public List<M> GetAll(FilterDefinition<M> filter = null) =>
            (filter != null ? Items.Find(filter) : Items.Find(User => true)).ToList();

        public I Get<I>(string Id, IBsonSerializer<I> serializer = null) where I : ModelBase => 
            Items.Find(M => M.Id == Id).As(serializer).FirstOrDefault();

        public M Get(string Id) =>
            Items.Find(M => M.Id == Id).FirstOrDefault();

        public M Create(M M)
        {
            if (Template.IsSchematic(M))
            {
                Items.InsertOne(M);
                return M;
            }

            return default;
        }
    }
}