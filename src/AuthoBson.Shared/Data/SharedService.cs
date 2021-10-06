using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Bson;
using AuthoBson.Shared.Data.Models;

namespace AuthoBson.Shared.Data {
    public class SharedService<Item> where Item : IModelBase {
        private IMongoCollection<Item> Items { get; set; }

        public IEnumerable<Item> GetAll() => Items.Find<Item>(Item => true).ToEnumerable();

        public Item GetItem(long Id) => Items.Find<Item>(Item => Item.Id == Id).FirstOrDefault();
    }
}