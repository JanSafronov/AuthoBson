using System;
using System.Collections;
using System.Collections.Generic;
using AuthoBson.Messaging.Services.Shared;
using AuthoBson.Shared.Data;

namespace AuthoBson.Messaging.Services
{
    public class MessageService : SharedService<Message>
    {
        public string getMessage(string Id) => this.GetItem(Id);

        public string PostMessage(Message message) => this.CreateItem(message);
    }
}