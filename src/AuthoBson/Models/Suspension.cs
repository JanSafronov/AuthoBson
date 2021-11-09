using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthoBson.Models
{
    public interface ISuspension
    {
        [BsonElement("Reason")]
        [JsonProperty("Reason")]
        [BsonRepresentation(BsonType.String)]
        String Reason { get; set; }

        [BsonElement("Duration")]
        [JsonProperty("Duration")]
        [BsonRepresentation(BsonType.DateTime)]
        DateTime Duration { get; set; }
    }

    [BsonDiscriminator("Suspension")]
    [Serializable]
    public class Suspension
    {
        public String Reason { get; set; }

        public DateTime Duration { get; set; }

        [BsonConstructor("Reason", "Duration")]
        public Suspension(string Reason, DateTime Duration)
        {
            this.Reason = Reason;
            this.Duration = Duration;
        }

        public Suspension() :
            this("Default", DateTime.Now)
        { }
    }
}
