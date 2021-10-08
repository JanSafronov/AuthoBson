using System;
using AuthoBson.Shared.Data.Models;

namespace AuthoBson.Messaging.Data.Models.Templates {
    public interface IMessageTemplate : IModelTemplate {
        int[] Header { get; set; }

        int[] Body { get; set; }

        bool Scheme(int[] Header, int[] Body);

        bool IsSchematic(Message Message);

        bool IsSchematic(string Header, string Body);
    }
    
    public class MessageTemplate : IMessageTemplate, IModelTemplate {
        public int[] Header { get; set; }

        public int[] Body { get; set; }

        public bool Scheme(int[] Header, int[] Body) { 
            return Header.Length == 2 && Body.Length == 2
                   && Header[0] < Header[1] && Body[0] < Body[1];
        }

        public bool IsSchematic(Message Message) =>
        this.Header[0] <= Message.Header.Length && Message.Header.Length < this.Header[1]
        && this.Body[0] <= Message.Body.Length && Message.Body.Length < this.Body[1];

        public bool IsSchematic(string Header, string Body) => 
        this.Header[0] <= Header.Length && Header.Length < this.Header[1]
        && this.Body[0] <= Body.Length && Body.Length < this.Body[1];

        public MessageTemplate(int[] Header, int[] Body) {
            if (Scheme(Header, Body)) {
                this.Header = Header;
                this.Body = Body;
            }
        }
    }
}