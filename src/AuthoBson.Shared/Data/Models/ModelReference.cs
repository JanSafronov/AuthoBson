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
        [BsonRepresentation(BsonType.String)]
        public string Route { get; set; }

        [BsonConstructor("Id", "Route")]
        public ModelReference(string Id, string Route)
        {
            this.Id = Id;
            this.Route = Route;
        }
    }
}
