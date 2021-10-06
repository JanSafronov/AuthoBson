using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Bson;

namespace AuthoBson.Shared.Data {
    public interface ISharedService<Item> where Item : ISharedModel {
        IMongoCollection<Item> Items { get; set; }

        IEnumerable<Item> GetAll() => Items.Find<Item>(Item => true).ToEnumerable();

        Item GetI(string Id) => Items.Find<Item>(Item => Item.Id == Id).FirstOrDefault();

        Item CreateItem(Item item);
    }
}