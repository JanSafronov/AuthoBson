using System; 
using AuthoBson.Shared.Data.Models;

namespace AuthoBson.Messaging.Data.Models.Templates {
    
    public class MessageTemplate : IModelTemplate<Message> {
        public int[] Header { get; set; }

        public int[] Body { get; set; }

        public static bool Scheme(int[] Header, int[] Body) =>
            Scheme(Header, Body);

        public static bool Scheme(MessageTemplate template) =>
            Scheme(template.Header, template.Body);

        public bool IsSchematic(string Header, string Body) =>
            this.Header[0] <= Header.Length && Header.Length < this.Header[1]
            && this.Body[0] <= Body.Length && Body.Length < this.Body[1];

        public override bool IsSchematic(Message Message) =>
            IsSchematic(Message.Header, Message.Body);
    }
}