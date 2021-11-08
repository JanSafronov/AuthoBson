﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthoBson.Shared.Data.Models;
using AuthoBson.Messaging.Data.Models;

namespace AuthoBson.Messaging.Data.Models.Templates
{

    public class ThreadTemplate : IModelTemplate
    {
        public MessageTemplate Content { get; set; }

        public MessageTemplate Message { get; set; }

        public static bool Scheme(ThreadTemplate template) =>
            Scheme(template.Content, template.Message);

        public static bool Scheme(MessageTemplate Content, MessageTemplate Message) =>
            MessageTemplate.Scheme(Content) && MessageTemplate.Scheme(Message);

        public bool IsSchematic(IThread Thread) =>
            IsSchematic(Thread.Content, Thread.Message)

        public bool IsSchematic(Message Content, Message Message) =>
            this.Content.IsSchematic(Content) && this.Message.IsSchematic(Message);
    }
}
