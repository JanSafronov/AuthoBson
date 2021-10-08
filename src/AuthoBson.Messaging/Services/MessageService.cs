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
    public class MessageService : SharedService<Message>
    {
        private IMongoCollection<Message> Items { get; set; }

        private IMessageTemplate Template { get; set; }

        public MessageService(IUserstoreDatabaseSettings settings, IMessageTemplate template) :
        base(settings, template) { }

        public MessageService(IUserstoreDatabase settings, IMessageTemplate template) :
        base(settings, template) { }

        public Message getMessage(string Id) => this.Get(Id);

        public string CreateMessage(Message message) {
            if (Template.Scheme())
        }
    }
}