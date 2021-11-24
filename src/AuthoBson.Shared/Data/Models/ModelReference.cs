using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using AuthoBson.Shared.Data.Models;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace AuthoBson.Shared.Data.Models
{
    public class ModelReference : IModelBase
    {
        public string Id { get; set; }

        [BsonElement("Route")]
        [JsonProperty("Route")]
        public KeyValuePair<string, string> Route { get; }

        [BsonConstructor("Id", "Route")]
        public ModelReference(string Id, KeyValuePair<string, string> Route)
        {
            this.Id = Id;
            this.Route = Route;
        }
    }
}

