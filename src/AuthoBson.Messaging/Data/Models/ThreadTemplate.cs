using System;
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
            IsSchematic(Thread.Content, Thread.Messages);

        public bool IsSchematic(Message Content, IEnumerable<Message> Messages) =>
            Messages.All(message => this.Message.IsSchematic(message)) &
            this.Content.IsSchematic(Content);
    }
}
