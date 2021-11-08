using System; 
using AuthoBson.Shared.Data.Models;

namespace AuthoBson.Messaging.Data.Models.Templates {
    
    public class MessageTemplate : IModelTemplate {
        public int[] Header { get; set; }

        public int[] Body { get; set; }

        public static bool Scheme(MessageTemplate template) =>
            Scheme(template.Header, template.Body);

        public static bool Scheme(int[] Header, int[] Body) =>
            Header.Length == 2 && Body.Length == 2
            && Header[0] < Header[1] && Body[0] < Body[1];

        public bool IsSchematic(IMessage Message) =>
            IsSchematic(Message.Header, Message.Body);

        public bool IsSchematic(string Header, string Body) => 
        this.Header[0] <= Header.Length && Header.Length < this.Header[1]
        && this.Body[0] <= Body.Length && Body.Length < this.Body[1];
    }
}