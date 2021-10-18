using System;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthoBson.Shared.Data.Models
{
    public interface IModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        string Id { get; }

        
        /*DateTime CreatedAt { get; set; }
        DateTime ModifiedAt { get; set; }*/
    }
}
