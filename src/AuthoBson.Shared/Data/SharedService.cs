using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Bson;

namespace AuthoBson.Shared.Data {
    public class SharedService<Item> where Item : ISharedModel {
        private IMongoCollection<Item> Items { get; set; }

        public IEnumerable<Item> GetAll() => Items.Find<Item>(Item => true).ToEnumerable();

        public Item GetItem(string Id) => Items.Find<Item>(Item => Item.Id == Id).FirstOrDefault();

        public abstract Item CreateItem(Item item);
    }
}