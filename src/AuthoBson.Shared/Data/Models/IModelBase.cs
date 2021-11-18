using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthoBson.Shared.Data.Models
{
    public interface IModelBase
    {
        string Id { get; }
    }
}
