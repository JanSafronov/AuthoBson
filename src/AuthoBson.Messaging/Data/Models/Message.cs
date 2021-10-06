using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Linq;
using AuthoBson.Shared.Data.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace AuthoBson.Messaging.Data.Models {
    public class Message : ModelBase {

        [BsonElement("Header")]
        [JsonProperty("Header")]
        [BsonRepresentation(BsonType.String)]
        public string Header { get; set; }

        [BsonElement("Body")]
        [JsonProperty("Body")]
        [BsonRepresentation(BsonType.String)]
        public string Body { get; set; }

        [BsonConstructor("Header", "Body")]
        public Message(string Header, string Body) {
            this.Header = Header;
            this.Body = Body;
        }
    }
}