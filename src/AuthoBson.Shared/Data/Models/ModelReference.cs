using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using AuthoBson.Shared.Data.Models;
using MongoDB.Bson.Serialization.Attributes;

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
