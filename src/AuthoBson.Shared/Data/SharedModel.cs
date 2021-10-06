using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthoBson.Shared.Data {
    public interface ISharedModel {

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        string Id { get; }
    }

    public class SharedModel {

    }
}