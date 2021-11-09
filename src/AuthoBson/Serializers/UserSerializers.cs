using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using AuthoBson.Models;

namespace AuthoBson.Serializers
{
    public class UserBsonSerializer : IBsonSerializer<User> {
        public User Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) {
            if (context.Reader != null)
            {
                BsonDocument bdoc = BsonSerializer.Deserialize<BsonDocument>(context.Reader);
                Console.WriteLine(bdoc);
                return new User(
                    Username: bdoc["Username"].AsString,
                    Password: bdoc["Password"].AsString,
                    Email: bdoc["Email"].AsString,
                    Notification: bdoc["Notification"].AsBoolean,
                    Verified: bdoc["Verified"].AsString,
                    Joined: bdoc["Joined"].ToUniversalTime(),
                    Role: (Role)bdoc["Role"].AsInt32,
                    Suspension: new Suspension(bdoc["Suspension"].AsBsonDocument["Reason"].AsString, bdoc["Suspension"].AsBsonDocument["Duration"].ToUniversalTime())
                );
            }
            return null;
        }
        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return null;
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, User obj) {
            context.Writer.WriteString(obj.Email);
        }
        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object obj) {
            if (obj is User user)
            {
                context.Writer.WriteString(user.Email);
            }
            else
            {
                throw new NotSupportedException("This is not an email");
            }
        }

        public static UserBsonSerializer Instance { get => new(); }

        public Type ValueType => typeof(User);
    }
}
