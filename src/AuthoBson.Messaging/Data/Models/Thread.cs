using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using AuthoBson.Messaging.Data.Models;
using AuthoBson.Shared;
using AuthoBson.Shared.Data.Models;

namespace AuthoBson.Messaging.Data.Models
{
    [MessageIn(typeof(Thread))]
    public interface IThread : IModelBase
    {
        Message Content { get; set; }

        DateTime CreatedAt { get; }

        IEnumerable<Message> Messages { get; set; }
    }

    public class Thread : ModelBase, IThread
    {
        [BsonElement("Content")]
        [JsonProperty("Content")]
        public Message Content { get; set; }

        [BsonElement("CreatedAt")]
        [JsonProperty("CreatedAt")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAt { get; }

        [BsonElement("Messages")]
        [JsonProperty("Messages")]
        public IEnumerable<Message> Messages { get; set; }

        [BsonConstructor("Content", "Messages")]
        public Thread(Message Content, IEnumerable<Message> Messages) :
        base()
        {
            this.Content = Content;
            this.Messages = Messages;
        }
    }
}
