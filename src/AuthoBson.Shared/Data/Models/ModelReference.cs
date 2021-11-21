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

        [BsonElement("Route")]
        [BsonRepresentation(BsonType.Document)]
        public KeyValuePair<string, string> Route { get; set; }

        [BsonConstructor("Id", "Route")]
        public ModelReference(string Id, KeyValuePair<string, string> Route)
        {
            this.Id = Id;
            this.Route = Route;
        }
    }
}
