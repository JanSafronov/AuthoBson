using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthoBson.Shared.Data.Models;

namespace AuthoBson.Messaging.Data.Models
{

    public class ThreadTemplate : IModelTemplate<Thread>
    {
        public MessageTemplate Content { get; set; }

        public MessageTemplate Message { get; set; }

        public static bool Scheme(MessageTemplate Content, MessageTemplate Message) =>
            MessageTemplate.Scheme(Content) && MessageTemplate.Scheme(Message);

        public static bool Scheme(ThreadTemplate template) =>
            Scheme(template.Content, template.Message);

        public bool IsSchematic(Message Content, IEnumerable<Message> Messages) =>
            Messages.All(message => Message.IsSchematic(message)) &
            this.Content.IsSchematic(Content);

        public override bool IsSchematic(Thread Thread) =>
            IsSchematic(Thread.Content, Thread.Messages);
    }
}
