using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Linq;
using AuthoBson.Shared;
using AuthoBson.Shared.Data.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;

namespace AuthoBson.Messaging.Data.Models {
    
    public interface IMessage : IModelBase {
        [BsonElement("Sender")]
        [JsonProperty("Sender")]
        ModelReference Sender { get; set; }

        [BsonElement("Receiver")]
        [JsonProperty("Receiver")]
        ModelReference Receiver { get; set; }

        [BsonElement("Header")]
        [JsonProperty("Header")]
        [BsonRepresentation(BsonType.String)]
        string Header { get; set; }

        [BsonElement("Body")]
        [JsonProperty("Body")]
        [BsonRepresentation(BsonType.String)]
        string Body { get; set; }
    }

    [BsonDiscriminator("Message")]
    public class Message : ModelBase, IMessage {
        public ModelReference Sender { get; set; }

        public ModelReference Receiver { get; set; }

        public string Header { get; set; }

        public string Body { get; set; }

        [BsonConstructor("Sender", "Receiver", "Header", "Body")]
        public Message(ModelReference Sender, ModelReference Receiver, string Header, string Body) :
        base() 
        {
            this.Sender = Sender;
            this.Receiver = Receiver;
            this.Header = Header;
            this.Body = Body;
        }
        
    }
}