using System;
using System.Collections;
using System.Collections.Generic;
using AuthoBson.Shared.Data.Models;

namespace AuthoBson.Shared {
    [AttributeUsage(AttributeTargets.Class ^ AttributeTargets.Interface, Inherited = true, AllowMultiple = true)]
    public class MessageInAttribute : Attribute
    {
        readonly ICollection<Type> exceptions;

        public MessageInAttribute(params Type[] exceptions)
        {
            this.exceptions = exceptions;
        }
    }

    [AttributeUsage(AttributeTargets.Class ^ AttributeTargets.Interface, Inherited = true, AllowMultiple = true)]
    public class MessageOutAttribute : Attribute
    {
        readonly ICollection<Type> exceptions;

        public MessageOutAttribute(params Type[] exceptions)
        {
            this.exceptions = exceptions;
        }
    }
}