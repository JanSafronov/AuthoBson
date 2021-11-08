using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Bson;
using AuthoBson.Shared.Data.Models;

namespace AuthoBson.Shared.Services {
    public abstract class SharedService<Item> where Item : IModelBase {
        IMongoCollection<Item> Items { get; set; }

        IModelTemplate Template { get; set; }

        public SharedService(IStoreDatabaseSettings settings, IModelTemplate template) {
            MongoClient client = new(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            Items = database.GetCollection<Item>(settings.CollectionName);

            Template = template;
        }

        public SharedService(IStoreDatabase settings, IModelTemplate template) {
            MongoClient client = new();
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            Items = database.GetCollection<Item>(settings.CollectionName);

            Template = template;
        }

        public IEnumerable<Item> GetAll() => Items.Find<Item>(Item => true).ToEnumerable();

        public Item GetItem(string Id) => Items.Find<Item>(Item => Item.Id == Id).FirstOrDefault();
    }
}