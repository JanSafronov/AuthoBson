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
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("SenderId")]
        [JsonProperty("SenderId")]
        string SenderId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("ReceiverId")]
        [JsonProperty("ReceiverId")]
        string ReceiverId { get; set; }

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
    public class Message : IMessage {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("SenderId")]
        [JsonProperty("SenderId")]
        public string SenderId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("ReceiverId")]
        [JsonProperty("ReceiverId")]
        public string ReceiverId { get; set; }

        [BsonElement("Header")]
        [JsonProperty("Header")]
        [BsonRepresentation(BsonType.String)]
        public string Header { get; set; }

        [BsonElement("Body")]
        [JsonProperty("Body")]
        [BsonRepresentation(BsonType.String)]
        public string Body { get; set; }

        [BsonConstructor("SenderId", "ReceiverId", "Header", "Body")]
        public Message(string SenderId, string ReceiverId, string Header, string Body) {
            Id = ObjectId.GenerateNewId().ToString();
            this.SenderId = SenderId;
            this.ReceiverId = ReceiverId;
            this.Header = Header;
            this.Body = Body;
        }

        [BsonConstructor("Sender", "Receiver", "Header", "Body")]
        public Message([Messaging] ModelBase Sender, [Messaging(true)] ModelBase Receiver, string Header, string Body)
        {
            Id = ObjectId.GenerateNewId().ToString();
            SenderId = Sender.Id;
            ReceiverId = Receiver.Id;
            this.Header = Header;
            this.Body = Body;
        }
    }
}