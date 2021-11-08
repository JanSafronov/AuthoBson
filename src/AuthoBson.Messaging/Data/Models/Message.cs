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

namespace AuthoBson.Messaging.Data.Models {
    
    public interface IMessage : IModelBase {
        [BsonElement("Sender")]
        [JsonProperty("Sender")]
        ModelReference Sender { get; }

        [BsonElement("Receiver")]
        [JsonProperty("Receiver")]
        ModelReference Receiver { get; }

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
        [BsonElement("Sender")]
        [JsonProperty("Sender")]
        public ModelReference Sender { get; }

        [BsonElement("Receiver")]
        [JsonProperty("Receiver")]
        public ModelReference Receiver { get; }

        [BsonElement("Header")]
        [JsonProperty("Header")]
        [BsonRepresentation(BsonType.String)]
        public string Header { get; set; }

        [BsonElement("Body")]
        [JsonProperty("Body")]
        [BsonRepresentation(BsonType.String)]
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

        [BsonConstructor("Sender", "Receiver", "Header", "Body")]
        public Message([MessageOut] ModelBase Sender, [MessageIn] ModelBase Receiver, string Header, string Body) :
        base()
        {
            this.Sender = new ModelReference(Sender.Id, Sender);
            this.Receiver = new ModelReference(Receiver.Id, Receiver);
            this.Header = Header;
            this.Body = Body;
        }
    }
}