using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
//using Microsoft.Extensions.DependencyInjection;

namespace AuthoBson.Shared.Data.Models
{
    public class ModelReference : IModelBase
    {
        public string Id { get; set; }

        [BsonElement("Type")]
        [BsonRepresentation(BsonType.String)]
        public string Type { get; set; }

        [BsonConstructor("Id", "Type")]
        public ModelReference(string Id, string Type)
        {
            this.Id = Id;
            this.Type = Type;
        }
    }
}
