using System;
using AuthoBson.Shared.Data.Models;

namespace AuthoBson.Messaging.Data.Models.Templates {
    public interface IMessageTemplate : IModelTemplate {
        int[] Header { get; set; }

        int[] Body { get; set; }

        bool Scheme(int[] Header, int[] Body);
    }
    
    public class MessageTemplate : IMessageTemplate, IModelTemplate {
        public int[] Header { get; set; }

        public int[] Body { get; set; }

        public bool Scheme(int[] Header, int[] Body) { 
            return Header.Length == 2 && Body.Length == 2
                   && Header[0] < Header[1] && Body[0] < Body[1];
        }

        public MessageTemplate(int[] Header, int[] Body) {
            if (Scheme(Header, Body)) {
                this.Header = Header;
                this.Body = Body;
            }
        }

        public MessageTemplate(Message Message) {
            if (Scheme(Message.Header, Message.Body)) {
                this.Header = Header;
                this.Body = Body;
            }
        }
    }
}