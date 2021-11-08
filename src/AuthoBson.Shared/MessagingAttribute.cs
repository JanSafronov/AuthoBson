using System;

namespace AuthoBson.Shared {
    [AttributeUsage(System.AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class MessagingAttribute : Attribute
    {
        readonly bool messageIn;
    
        public MessagingAttribute(bool messageIn = false)
        {
            this.messageIn = messageIn;
        }
    
        public bool MessageIn
        {
            get => messageIn;
        }
    
        public int NamedInt { get; set; }
    } 
}