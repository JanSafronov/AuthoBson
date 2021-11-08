using System;

namespace AuthoBson.Shared {
    [AttributeUsage(System.AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class MessageInAttribute : Attribute
    {
        readonly bool messageIn;
    
        public bool MessageIn
        {
            get => messageIn;
        }
    
        public int NamedInt { get; set; }
    }

    [AttributeUsage(System.AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class MessageOutAttribute : Attribute
    {
        readonly bool messageOut;

        public bool MessageOut
        {
            get => messageOut;
        }

        public int NamedInt { get; set; }
    }
}