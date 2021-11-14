using System;
using System.Collections;
using System.Collections.Generic;
using AuthoBson.Messaging.Data.Models;
using AuthoBson.Messaging.Data.Models.Templates;
using AuthoBson.Shared.Data;
using AuthoBson.Shared.Data.Models;
using AuthoBson.Shared.Services;
using AuthoBson.Shared.Results;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AuthoBson.Messaging.Services
{
    public class MessageService : SharedService<Message>
    {
        private IMongoCollection<Message> Messages { get; set; }

        private MessageTemplate Template { get; set; }

        public MessageService(IStoreDatabaseSettings settings, MessageTemplate template) :
            base(settings, template)
        { }

        public MessageService(IStoreDatabase settings, MessageTemplate template) :
            base(settings, template)
        { }

        /// <summary>
        /// Returns by conditional Ids the list of all messages
        /// </summary>
        /// <param name="senderId">Id of the sender</param>
        /// <param name="receiverId">Id of the receiver</param>
        /// <returns>List of all messagess by optional Ids</returns>
        public List<Message> GetAll(string senderId = null, string receiverId = null) => Messages.Find(Message =>
            senderId != null ? Message.Receiver.Id == senderId :
            receiverId == null || Message.Sender.Id == receiverId).ToList();

        /// <summary>
        /// Find's the message by it's Id
        /// </summary>
        /// <param name="id">Id of the message to find</param>
        /// <returns>Found message or null</returns>
        public Message GetMessage(string Id) =>
            base.Get<Message>(Id);

        /// <summary>
        /// Creates a new message in the database's collection
        /// </summary>
        /// <param name="User">The message to insert in the database's collection</param>
        /// <returns>The inserted message</returns>
        public Message CreateMessage(Message Message) =>
            base.Create(Message);
    }
}
