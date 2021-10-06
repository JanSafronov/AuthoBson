using System;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthoBson.Shared.Data.Models
{
    public interface IModelBase
    {
        [BsonId]
        [BsonElement("Id")]
        long Id { get; set; }

        
        /*DateTime CreatedAt { get; set; }
        DateTime ModifiedAt { get; set; }*/
    }
}