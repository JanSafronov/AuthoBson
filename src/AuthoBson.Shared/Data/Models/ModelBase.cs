using System;
using MongoDB.Bson;

namespace AuthoBson.Shared.Data.Models
{
    public abstract class ModelBase : IModelBase
    {
        public string Id { get; }

        public ModelBase()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }
    }
}