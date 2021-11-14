﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthoBson.Shared.Data.Models
{
    [BsonDiscriminator("ModelBase")]
    public class ModelBase : IModelBase
    {
        [Unique("Id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; }

        public ModelBase() :
            this(ObjectId.GenerateNewId().ToString())
        { }

        public ModelBase(string Id)
        {
            this.Id = Id;
        }
    }
}
