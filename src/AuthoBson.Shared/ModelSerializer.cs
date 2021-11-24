using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using AuthoBson.Shared.Data.Models;

namespace AuthoBson.Shared
{
    public class ModelBsonSerializer : IBsonSerializer<ModelBase>
    {
        public ModelBase Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context.Reader != null)
            {
                BsonDocument bdoc = BsonSerializer.Deserialize<BsonDocument>(context.Reader);
                Console.WriteLine(bdoc);
                return new ModelBase(
                    Id: bdoc["_id"].AsObjectId.ToString()
                );
            }
            return null;
        }
        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return null;
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, ModelBase obj)
        {
            context.Writer.WriteString(obj.Id);
        }
        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object obj)
        {
            if (obj is ModelBase model)
            {
                context.Writer.WriteString(model.Id);
            }
            else
            {
                throw new NotSupportedException("This is not a model base");
            }
        }

        public static ModelBsonSerializer Instance { get => new(); }

        public Type ValueType => typeof(ModelBase);
    }
}
