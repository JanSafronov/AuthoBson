using System;
using System.Collections;
using System.Collections.Generic;
using AuthoBson.Messaging.Services.Shared;
using AuthoBson.Messaging.Data.Models;
using AuthoBson.Messaging.Data.Models.Templates;
using AuthoBson.Shared.Data;
using AuthoBson.Shared.Data.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AuthoBson.Messaging.Services
{
    public class MessageService
    {
        private IMongoCollection<Message> Messages { get; set; }

        private IMessageTemplate Template { get; set; }

        public MessageService(IUserstoreDatabaseSettings settings, IMessageTemplate template) {
            MongoClient client = new(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            Messages = database.GetCollection<Message>(settings.UsersCollectionName);

            Template = template;
        }

        public MessageService(IUserstoreDatabase settings, IMessageTemplate template) {
            MongoClient client = new();
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            Messages = database.GetCollection<Message>(settings.UsersCollectionName);

            Template = template;
        }

        public List<Message> GetAll(string senderId = null, string receiverId = null) => Messages.Find(Message =>
            senderId != null ? Message.SenderId == senderId :
            receiverId != null ? Message.ReceiverId == receiverId : true).ToList();

        public Message GetMessage(string Id) => Messages.Find(Message => Message.Id == Id).As<Message>().FirstOrDefault();

        public Message CreateMessage(Message Message) {
            if (Template.IsSchematic(Message)) {
                Messages.InsertOne(Message);
                return Message;
            }
            return null;
        }
    }
}