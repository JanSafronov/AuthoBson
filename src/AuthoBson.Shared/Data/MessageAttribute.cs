using System;

namespace AuthoBson.Shared.Data {
    [System.AttributeUsage(System.AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class MessagingAttribute : Attribute
    {
        // See the attribute guidelines at
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly bool messageIn;
    
        // This is a positional argument
        public MessagingAttribute(bool messageIn = false)
        {
            this.messageIn = messageIn;
        
            // TODO: Implement code here
            throw new System.NotImplementedException();
        }
    
        public bool MessageIn
        {
            get => messageIn;
        }
    
        // This is a named argument
        public int NamedInt { get; set; }
    } 
}