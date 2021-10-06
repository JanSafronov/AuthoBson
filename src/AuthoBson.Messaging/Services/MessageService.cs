using System;
using System.Collections;
using System.Collections.Generic;
using AuthoBson.Messaging.Services.Shared;
using AuthoBson.Messaging.Data.Models;
using AuthoBson.Shared.Data;

namespace AuthoBson.Messaging.Services
{
    public class MessageService : SharedService<Message>
    {
        public Message getMessage(long Id) => this.GetItem(Id);

        public string PostMessage(Message message) => this.CreateItem(message);
    }
}