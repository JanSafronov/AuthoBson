using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace AuthoBson.Shared.Data.Models
{
    public abstract class ModelBase : IModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public ModelBase() :
            this(ObjectId.GenerateNewId().ToString())
        {
        }

        public ModelBase(string Id)
        {
            this.Id = Id;
        }
    }
}