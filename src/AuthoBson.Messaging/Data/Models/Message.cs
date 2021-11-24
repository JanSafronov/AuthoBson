using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Linq;
using AuthoBson.Shared;
using AuthoBson.Shared.Attributes;
using AuthoBson.Shared.Data.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;

namespace AuthoBson.Messaging.Data.Models {
    
    public interface IMessage : IModelBase {
        ModelReference Sender { get; set; }

        ModelReference Receiver { get; set; }

        string Header { get; set; }

        string Body { get; set; }
    }

    [BsonDiscriminator("Message")]
    [MessageIn]
    public class Message : ModelBase, IMessage {
        [BsonElement("Sender")]
        [JsonProperty("Sender")]
        public ModelReference Sender { get; set; }

        [BsonElement("Receiver")]
        [JsonProperty("Receiver")]
        public ModelReference Receiver { get; set; }

        [BsonElement("Header")]
        [JsonProperty("Header")]
        [BsonRepresentation(BsonType.String)]
        public string Header { get; set; }

        [BsonElement("Body")]
        [JsonProperty("Body")]
        [BsonRepresentation(BsonType.String)]
        public string Body { get; set; }

        [BsonElement("CreatedAt")]
        [JsonProperty("CreatedAt")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [BsonConstructor("Sender", "Receiver", "Header", "Body")]
        [JsonConstructor]
        public Message(ModelReference Sender, ModelReference Receiver, string Header, string Body) :
        base() 
        {
            this.Sender = Sender;
            this.Receiver = Receiver;
            this.Header = Header;
            this.Body = Body;
            this.CreatedAt = DateTime.Now;
        }
        
    }
}
